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
using DustInTheWind.DirectoryCompare.Domain.PotModel;
using DustInTheWind.DirectoryCompare.Ports.DataAccess;
using DustInTheWind.DirectoryCompare.Ports.FileSystemAccess;
using DustInTheWind.DirectoryCompare.Ports.LogAccess;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Cli.Application.SnapshotArea.CreateSnapshot;

public class CreateSnapshotUseCase : IRequestHandler<CreateSnapshotRequest, IDiskAnalysisProgress>
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

    public async Task<IDiskAnalysisProgress> Handle(CreateSnapshotRequest request, CancellationToken cancellationToken)
    {
        Pot pot = await RetrievePot(request);
        IDiskAnalysisProgress progress = await StartPathAnalysis(pot);

        return progress;
    }

    private async Task<Pot> RetrievePot(CreateSnapshotRequest request)
    {
        Pot pot = await potRepository.GetByNameOrId(request.PotName);

        if (pot == null)
            throw new PotDoesNotExistException(request.PotName);

        return pot;
    }

    private async Task<DiskAnalysis.DiskAnalysis> StartPathAnalysis(Pot pot)
    {
        log.WriteInfo("Scanning path: {0}", pot.Path);

        DiskAnalysis.DiskAnalysis diskAnalysis = new(fileSystem)
        {
            RootPath = pot.Path,
            SnapshotWriter = await snapshotRepository.CreateWriter(pot.Name),
            BlackList = await blackListRepository.Get(pot.Name)
        };

        diskAnalysis.Starting += HandleDiskReaderStarting;
        diskAnalysis.ErrorEncountered += HandleDiskReaderErrorEncountered;
        diskAnalysis.Finished += HandleDiskAnalysisFinished;

        await diskAnalysis.Run();

        return diskAnalysis;
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