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

using DustInTheWind.DirectoryCompare.DataStructures;
using DustInTheWind.DirectoryCompare.Domain.Comparison;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Ports.DataAccess;
using DustInTheWind.DirectoryCompare.Ports.LogAccess;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Cli.Application.MiscellaneousArea.FindDuplicates;

public class FindDuplicatesUseCase : IRequestHandler<FindDuplicatesRequest, FindDuplicatesResponse>
{
    private readonly ISnapshotRepository snapshotRepository;
    private readonly IBlackListRepository blackListRepository;
    private readonly ILog log;

    public FindDuplicatesUseCase(ISnapshotRepository snapshotRepository, IBlackListRepository blackListRepository, ILog log)
    {
        this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
        this.blackListRepository = blackListRepository ?? throw new ArgumentNullException(nameof(blackListRepository));
        this.log = log ?? throw new ArgumentNullException(nameof(log));
    }

    public async Task<FindDuplicatesResponse> Handle(FindDuplicatesRequest request, CancellationToken cancellationToken)
    {
        log.WriteInfo("Searching for duplicates between pot '{0}' and '{1}'.", request.SnapshotLeft.PotName, request.SnapshotRight.PotName);

        FileDuplicates fileDuplicates = new()
        {
            FilesLeft = await GetFiles(request.SnapshotLeft),
            FilesRight = string.IsNullOrEmpty(request.SnapshotRight.PotName)
                ? null
                : await GetFiles(request.SnapshotRight),
            CheckFilesExistence = request.CheckFilesExistence
        };

        return new FindDuplicatesResponse
        {
            DuplicatePairs = fileDuplicates.ToList().ToDto()
        };
    }

    private async Task<List<HFile>> GetFiles(SnapshotLocation snapshotLocation)
    {
        BlackList blackList = await GetBlackList(snapshotLocation);
        Snapshot snapshot = await snapshotRepository.Get(snapshotLocation);

        IEnumerable<HFile> files = snapshot == null
            ? Enumerable.Empty<HFile>()
            : snapshot.EnumerateFiles(snapshotLocation.InternalPath, blackList);

        return files.ToList();
    }

    private async Task<BlackList> GetBlackList(SnapshotLocation snapshotLocation)
    {
        if (snapshotLocation.PotName == null)
            return null;

        DiskPathCollection blackListPaths = await blackListRepository.Get(snapshotLocation.PotName);
        return new BlackList(blackListPaths);
    }
}