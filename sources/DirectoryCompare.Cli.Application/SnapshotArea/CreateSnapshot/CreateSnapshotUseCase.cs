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
using DustInTheWind.DirectoryCompare.Ports.DataAccess.ImportExport;
using DustInTheWind.DirectoryCompare.Ports.FileSystemAccess;
using DustInTheWind.DirectoryCompare.Ports.LogAccess;
using DustInTheWind.DirectoryCompare.Ports.SystemAccess;
using DustInTheWind.DirectoryCompare.Ports.UserAccess;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Cli.Application.SnapshotArea.CreateSnapshot;

public class CreateSnapshotUseCase : IRequestHandler<CreateSnapshotRequest>
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

    public async Task Handle(CreateSnapshotRequest request, CancellationToken cancellationToken)
    {
        Pot pot = await RetrievePot(request.PotName);
        DiskPathCollection blackList = await RetrieveBlackListPaths(pot);
        CheckPotPathExists(pot);
        await StartDiskAnalysis(pot, blackList);
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
        return blackList ?? new DiskPathCollection();
    }

    private void CheckPotPathExists(Pot pot)
    {
        log.WriteInfo($"Checking that pot path exists: '{pot.Path}'.");

        bool exists = fileSystem.ExistsDirectory(pot.Path);

        if (!exists)
            throw new PotPathDoesNotExistException(pot.Name, pot.Path);
    }

    private async Task StartDiskAnalysis(Pot pot, DiskPathCollection blackList)
    {
        await AnnounceStarting(pot, blackList);

        IDiskCrawler diskCrawler = CreateDiskCrawler(pot, blackList);
        PreAnalysis preAnalysis = await RunPreAnalysis(diskCrawler);
        using ISnapshotWriter snapshotWriter = await OpenSnapshotWriter(pot);

        DiskAnalysis.DiskAnalysis diskAnalysis = new(log, createSnapshotUi)
        {
            DiskCrawler = diskCrawler,
            PreAnalysis = preAnalysis,
            SnapshotWriter = snapshotWriter
        };

        await diskAnalysis.RunAsync();
    }

    private Task AnnounceStarting(Pot pot, DiskPathCollection blackList)
    {
        log.WriteInfo("Scanning path: {0}", pot.Path);

        if (blackList.Count == 0)
        {
            log.WriteInfo("No blacklist entries.");
            return Task.CompletedTask;
        }

        log.WriteInfo("Computed black list:");

        foreach (string blackListItem in blackList)
            log.WriteInfo("- " + blackListItem);

        return AnnounceSnapshotCreating(pot, blackList);
    }

    private Task AnnounceSnapshotCreating(Pot pot, DiskPathCollection blackList)
    {
        StartNewSnapshotInfo info = new()
        {
            PotName = pot.Name,
            Path = pot.Path,
            BlackList = blackList
                .Select(x => x.ToString())
                .ToList(),
            StartTime = systemClock.GetCurrentUtcTime()
        };

        return createSnapshotUi.AnnounceStarting(info);
    }

    private IDiskCrawler CreateDiskCrawler(Pot pot, DiskPathCollection blackList)
    {
        List<string> includeRules = pot.IncludedPaths
            .Select(x=> (string)x)
            .ToList();

        List<string> excludeRules = blackList.ToListOfStrings();

        return fileSystem.CreateCrawler(pot.Path, includeRules, excludeRules);
    }

    private async Task<PreAnalysis> RunPreAnalysis(IDiskCrawler diskCrawler)
    {
        PreAnalysis preAnalysis = new(diskCrawler, createSnapshotUi);
        await preAnalysis.RunAsync();

        return preAnalysis;
    }

    private Task<ISnapshotWriter> OpenSnapshotWriter(Pot pot)
    {
        return snapshotRepository.CreateWriter(pot.Name);
    }
}