// DirectoryCompare
// Copyright (C) 2017-2024 Dust in the Wind
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using DustInTheWind.Clindy.Applications.PresentDuplicates;
using DustInTheWind.Clindy.Applications.SetCurrentDuplicateGroup;
using DustInTheWind.DirectoryCompare.Infrastructure;
using DustInTheWind.DirectoryCompare.Ports.ConfigAccess;
using DustInTheWind.DirectoryCompare.Ports.FileSystemAccess;
using DustInTheWind.DirectoryCompare.Ports.ImportExportAccess;
using MediatR;

namespace DustInTheWind.Clindy.Applications.LoadDuplicates;

internal class LoadDuplicatesUseCase : IRequestHandler<LoadDuplicatesRequest>
{
    private readonly IImportExport importExport;
    private readonly IFileSystem fileSystem;
    private readonly ApplicationState applicationState;
    private readonly EventBus eventBus;
    private readonly IConfig config;

    public LoadDuplicatesUseCase(IImportExport importExport, IFileSystem fileSystem, ApplicationState applicationState,
        EventBus eventBus, IConfig config)
    {
        this.importExport = importExport ?? throw new ArgumentNullException(nameof(importExport));
        this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        this.applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));
        this.eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        this.config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public async Task Handle(LoadDuplicatesRequest request, CancellationToken cancellationToken)
    {
        RaiseDuplicatesListLoadingEvent();

        DuplicateGroup oldDuplicateGroup = applicationState.CurrentDuplicateGroup;
        applicationState.CurrentDuplicateGroup = null;
        RaiseCurrentDuplicateChangedEvent(null);

        DuplicateGroupCollection duplicatesGroupCollection = await RetrieveDuplicatesGroupCollection(cancellationToken);
        applicationState.Duplicates = duplicatesGroupCollection;

        if (oldDuplicateGroup != null)
        {
            bool oldDuplicateGroupStillExists = duplicatesGroupCollection.Any(x => x.FileHash == oldDuplicateGroup.FileHash);

            if (oldDuplicateGroupStillExists)
            {
                applicationState.CurrentDuplicateGroup = oldDuplicateGroup;
                RaiseCurrentDuplicateChangedEvent(oldDuplicateGroup);
            }
        }
        
        RaiseDuplicatesLoadedEvent();
    }

    private async Task<DuplicateGroupCollection> RetrieveDuplicatesGroupCollection(CancellationToken cancellationToken)
    {
        return await MinimumExecutionTime.RunAsync(1000, () =>
        {
            IEnumerable<DuplicateGroup> fileDuplicateGroups = RetrieveFileDuplicateGroups()
                .Select(x => new DuplicateGroup
                {
                    FilePaths = x.FilePaths,
                    FileSize = x.FileSize,
                    FileHash = x.FileHash
                });
            DuplicateGroupCollection duplicateGroupCollection = new(fileDuplicateGroups);
            return Task.FromResult(duplicateGroupCollection);
        }, cancellationToken);
    }

    private void RaiseDuplicatesListLoadingEvent()
    {
        DuplicatesLoadingEvent duplicatesLoadedEvent = new();
        eventBus.Publish(duplicatesLoadedEvent);
    }

    private IEnumerable<DuplicateGroupDto> RetrieveFileDuplicateGroups()
    {
        IDuplicatesInput duplicatesInput = importExport.OpenDuplicatesInput(config.DuplicatesFilePath);
        IEnumerable<DuplicateGroupDto> fileDuplicateGroups = duplicatesInput.EnumerateDuplicates();

        if (config.CheckFilesExistence)
        {
            fileDuplicateGroups = fileDuplicateGroups
                .Select(x =>
                {
                    x.FilePaths = x.FilePaths
                        .Where(filePath => fileSystem.FileExists(filePath))
                        .ToList();

                    return x;
                })
                .Where(x => x.FilePaths.Count > 1);
        }
        return fileDuplicateGroups;
    }

    private void RaiseDuplicatesLoadedEvent()
    {
        DuplicatesLoadedEvent duplicatesLoadedEvent = new();
        eventBus.Publish(duplicatesLoadedEvent);
    }

    private void RaiseCurrentDuplicateChangedEvent(DuplicateGroup duplicateGroup)
    {
        CurrentDuplicateGroupChangedEvent currentDuplicateGroupChanged = new()
        {
            DuplicateGroup = duplicateGroup
        };

        eventBus.Publish(currentDuplicateGroupChanged);
    }
}