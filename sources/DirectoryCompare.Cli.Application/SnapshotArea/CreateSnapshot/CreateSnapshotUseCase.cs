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
using DustInTheWind.DirectoryCompare.DataStructures;
using DustInTheWind.DirectoryCompare.Domain.PotModel;
using DustInTheWind.DirectoryCompare.Ports.DataAccess;
using DustInTheWind.DirectoryCompare.Ports.FileSystemAccess;
using DustInTheWind.DirectoryCompare.Ports.LogAccess;
using DustInTheWind.DirectoryCompare.Ports.SystemAccess;
using DustInTheWind.DirectoryCompare.Ports.UserAccess;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Cli.Application.SnapshotArea.CreateSnapshot;

public class CreateSnapshotUseCase : IRequestHandler<CreateSnapshotRequest, IDiskAnalysisReport>
{
    private readonly ILog log;
    private readonly IPotRepository potRepository;
    private readonly IBlackListRepository blackListRepository;
    private readonly ISnapshotRepository snapshotRepository;
    private readonly IFileSystem fileSystem;
    private readonly ICreateSnapshotUi createSnapshotUi;
    private readonly ISystemClock systemClock;

    public CreateSnapshotUseCase(ILog log, IPotRepository potRepository,
        IBlackListRepository blackListRepository, ISnapshotRepository snapshotRepository,
        IFileSystem fileSystem, ICreateSnapshotUi createSnapshotUi, ISystemClock systemClock)
    {
        this.log = log ?? throw new ArgumentNullException(nameof(log));
        this.potRepository = potRepository ?? throw new ArgumentNullException(nameof(potRepository));
        this.blackListRepository = blackListRepository ?? throw new ArgumentNullException(nameof(blackListRepository));
        this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
        this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        this.createSnapshotUi = createSnapshotUi ?? throw new ArgumentNullException(nameof(createSnapshotUi));
        this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
    }

    public async Task<IDiskAnalysisReport> Handle(CreateSnapshotRequest request, CancellationToken cancellationToken)
    {
        Pot pot = await RetrievePot(request.PotName);
        DiskPathCollection rootedBlackList = await RetrieveBlackListPaths(pot);
        CheckPotPathExists(pot);
        IDiskAnalysisReport report = StartDiskAnalysis(pot, rootedBlackList);

        return report;
    }

    private async Task<Pot> RetrievePot(string potName)
    {
        log.WriteInfo($"Retrieving pot: '{potName}'.");

        Pot pot = await potRepository.GetByNameOrId(potName);

        if (pot == null)
            throw new PotNotFoundException(potName);

        return pot;
    }

    private async Task<DiskPathCollection> RetrieveBlackListPaths(Pot pot)
    {
        log.WriteInfo("Building the black list paths.");

        DiskPathCollection blackList = await blackListRepository.Get(pot.Name);

        return blackList == null
            ? new DiskPathCollection()
            : blackList.PrependPath(pot.Path);
    }

    private void CheckPotPathExists(Pot pot)
    {
        log.WriteInfo($"Checking that pot path exists: '{pot.Path}'.");

        bool exists = fileSystem.ExistsDirectory(pot.Path);

        if (!exists)
            throw new PotPathDoesNotExistException(pot.Name, pot.Path);
    }

    private IDiskAnalysisReport StartDiskAnalysis(Pot pot, DiskPathCollection blackList)
    {
        DiskAnalysis.DiskAnalysis diskAnalysis = new(log, fileSystem, snapshotRepository, createSnapshotUi, systemClock)
        {
            Pot = pot,
            BlackList = blackList
        };

        return diskAnalysis.Start();
    }
}