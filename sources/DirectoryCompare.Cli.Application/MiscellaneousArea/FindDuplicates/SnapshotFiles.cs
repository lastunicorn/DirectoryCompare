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

using DustInTheWind.DirectoryCompare.DataStructures;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Ports.DataAccess;

namespace DustInTheWind.DirectoryCompare.Cli.Application.MiscellaneousArea.FindDuplicates;

internal class SnapshotFiles
{
    private readonly SnapshotLocation snapshotLocation;
    private readonly ISnapshotRepository snapshotRepository;
    private readonly IBlackListRepository blackListRepository;

    public SnapshotFiles(SnapshotLocation snapshotLocation, ISnapshotRepository snapshotRepository,
        IBlackListRepository blackListRepository)
    {
        this.snapshotLocation = snapshotLocation;
        this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
        this.blackListRepository = blackListRepository ?? throw new ArgumentNullException(nameof(blackListRepository));
    }

    public async Task<IEnumerable<HFile>> Enumerate()
    {
        if (string.IsNullOrEmpty(snapshotLocation.PotName))
            return null;

        Snapshot snapshot = await snapshotRepository.Get(snapshotLocation);

        if (snapshot == null)
            return Enumerable.Empty<HFile>();

        BlackList blackList = await GetBlackList(snapshotLocation.PotName);
        return snapshot.EnumerateFiles(snapshotLocation.InternalPath, blackList);
    }

    private async Task<BlackList> GetBlackList(string potName)
    {
        if (potName == null)
            return null;

        IEnumerable<IBlackItem> blackListPaths = (await blackListRepository.Get(potName))
            .Select(x => new PathBlackItem(x));

        IEnumerable<IBlackItem> blackListHashes = (await blackListRepository.GetDuplicateExcludes(potName))
            .Select(x => new FileHashBlackItem(x));

        IEnumerable<IBlackItem> blackListItems = blackListPaths.Concat(blackListHashes);
        return new BlackList(blackListItems);
    }
}