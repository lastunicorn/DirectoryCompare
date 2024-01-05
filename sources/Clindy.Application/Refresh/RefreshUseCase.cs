// DirectoryCompare
// Copyright (C) 2017-2023 Dust in the Wind
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

using DustInTheWind.Clindy.Applications.LoadDuplicates;
using DustInTheWind.DirectoryCompare.Ports.ConfigAccess;
using DustInTheWind.DirectoryCompare.Ports.FileSystemAccess;
using DustInTheWind.DirectoryCompare.Ports.ImportExportAccess;
using MediatR;

namespace DustInTheWind.Clindy.Applications.Refresh;

internal class RefreshUseCase : IRequestHandler<RefreshRequest>
{
    private readonly IImportExport importExport;
    private readonly IFileSystem fileSystem;
    private readonly ApplicationState applicationState;
    private readonly EventBus eventBus;
    private readonly IConfig config;

    public RefreshUseCase(IImportExport importExport, IFileSystem fileSystem, ApplicationState applicationState,
        EventBus eventBus, IConfig config)
    {
        this.importExport = importExport ?? throw new ArgumentNullException(nameof(importExport));
        this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        this.applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));
        this.eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        this.config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public async Task Handle(RefreshRequest request, CancellationToken cancellationToken)
    {
        await RaiseDuplicatesListLoadingEvent(cancellationToken);

        IEnumerable<FileDuplicateGroup> fileDuplicateGroups = RetrieveFileDuplicateGroups();
        applicationState.Duplicates = new DuplicateGroupCollection(fileDuplicateGroups);

        await RaiseDuplicatesListLoadedEvent(cancellationToken);
    }

    private async Task RaiseDuplicatesListLoadingEvent(CancellationToken cancellationToken)
    {
        DuplicatesListLoadingEvent duplicatesListLoadedEvent = new();
        await eventBus.PublishAsync(duplicatesListLoadedEvent, cancellationToken);
    }

    private IEnumerable<FileDuplicateGroup> RetrieveFileDuplicateGroups()
    {
        IDuplicatesInput duplicatesInput = importExport.OpenDuplicatesInput(config.DuplicatesFilePath);
        IEnumerable<FileDuplicateGroup> fileDuplicateGroups = duplicatesInput.EnumerateDuplicates();

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

    private async Task RaiseDuplicatesListLoadedEvent(CancellationToken cancellationToken)
    {
        DuplicatesListLoadedEvent duplicatesListLoadedEvent = new();
        await eventBus.PublishAsync(duplicatesListLoadedEvent, cancellationToken);
    }
}