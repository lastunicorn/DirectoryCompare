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

using DustInTheWind.DirectoryCompare.Cli.Application.SnapshotArea.CreateSnapshot.DiskAnalysis;
using DustInTheWind.DirectoryCompare.Cli.Application.Utils;
using DustInTheWind.DirectoryCompare.DataStructures;
using DustInTheWind.DirectoryCompare.Domain.PotModel;
using DustInTheWind.DirectoryCompare.Ports.DataAccess;
using DustInTheWind.DirectoryCompare.Ports.FileSystemAccess;
using DustInTheWind.DirectoryCompare.Ports.LogAccess;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Cli.Application.SnapshotArea.CreateSnapshot;

public class CreateSnapshotUseCase : IRequestHandler<CreateSnapshotRequest, IDiskAnalysisStateReport>
{
    private readonly ILog log;
    private readonly IPotRepository potRepository;
    private readonly IBlackListRepository blackListRepository;
    private readonly ISnapshotRepository snapshotRepository;
    private readonly IFileSystem fileSystem;

    public CreateSnapshotUseCase(ILog log, IPotRepository potRepository,
        IBlackListRepository blackListRepository, ISnapshotRepository snapshotRepository,
        IFileSystem fileSystem)
    {
        this.log = log ?? throw new ArgumentNullException(nameof(log));
        this.potRepository = potRepository ?? throw new ArgumentNullException(nameof(potRepository));
        this.blackListRepository = blackListRepository ?? throw new ArgumentNullException(nameof(blackListRepository));
        this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
        this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }

    public async Task<IDiskAnalysisStateReport> Handle(CreateSnapshotRequest request, CancellationToken cancellationToken)
    {
        Pot pot = await RetrievePot(request.PotName);
        CheckPotPathExists(pot.Path);
        DiskPathCollection rootedBlackList = await RetrieveBlackListPaths(pot);
        IDiskAnalysisStateReport stateReport = await StartPathAnalysis(pot, rootedBlackList);

        return stateReport;
    }

    private async Task<Pot> RetrievePot(string potName)
    {
        log.WriteInfo($"Retrieving pot: '{potName}'.");

        Pot pot = await potRepository.GetByNameOrId(potName);

        if (pot == null)
            throw new PotDoesNotExistException(potName);

        return pot;
    }

    private void CheckPotPathExists(DiskPath potPath)
    {
        log.WriteInfo($"Checking that pot path exists: '{potPath}'.");

        bool exists = fileSystem.ExistsDirectory(potPath);

        if (!exists)
            throw new Exception($"The path to scan does not exist: {potPath}");
    }

    private async Task<DiskPathCollection> RetrieveBlackListPaths(Pot pot)
    {
        log.WriteInfo("Building the black list paths.");

        DiskPathCollection blackList = await blackListRepository.Get(pot.Name);

        return blackList == null
            ? new DiskPathCollection()
            : blackList.PrependPath(pot.Path);
    }

    private async Task<IDiskAnalysisStateReport> StartPathAnalysis(Pot pot, DiskPathCollection diskPathCollection)
    {
        log.WriteInfo("Scanning path: {0}", pot.Path);

        DiskAnalysis.DiskAnalysis diskAnalysis = new(fileSystem)
        {
            RootPath = pot.Path,
            SnapshotWriter = await snapshotRepository.CreateWriter(pot.Name),
            BlackList = diskPathCollection
        };

        diskAnalysis.StateReport.Starting += HandleDiskReaderStarting;
        diskAnalysis.StateReport.ErrorEncountered += HandleDiskReaderErrorEncountered;
        diskAnalysis.StateReport.Finished += HandleDiskAnalysisFinished;

        _ = diskAnalysis.Run();

        return diskAnalysis.StateReport;
    }

    private void HandleDiskReaderStarting(object sender, DiskReaderStartingEventArgs e)
    {
        if (e.BlackList.Count == 0)
        {
            log.WriteInfo("No blacklist entries.");
            return;
        }

        log.WriteInfo("Computed black list:");

        foreach (string blackListItem in e.BlackList)
            log.WriteInfo("- " + blackListItem);
    }

    private void HandleDiskReaderErrorEncountered(object sender, ErrorEncounteredEventArgs e)
    {
        log.WriteError("Error while reading path '{0}': {1}", e.Path, e.Exception);
    }

    private void HandleDiskAnalysisFinished(object sender, EventArgs e)
    {
        if (sender is DiskAnalysis.DiskAnalysis diskAnalysis)
        {
            log.WriteInfo("Finished scanning path in {0}", diskAnalysis.ElapsedTime);
            diskAnalysis.SnapshotWriter.Dispose();
        }
    }
}