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

using DustInTheWind.DirectoryCompare.Domain;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Ports.DataAccess;

namespace DustInTheWind.DirectoryCompare.Cli.Application;

public class SnapshotFactory
{
    private readonly ISnapshotRepository snapshotRepository;

    public SnapshotFactory(ISnapshotRepository snapshotRepository)
    {
        this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
    }

    public Snapshot RetrieveSnapshot(SnapshotLocation location)
    {
        if (string.IsNullOrEmpty(location.PotName))
            throw new Exception("Pot name was not provided.");

        if (location.SnapshotIndex.HasValue)
            return snapshotRepository.GetByIndex(location.PotName, location.SnapshotIndex.Value);

        if (location.SnapshotDate.HasValue)
        {
            DateTime searchedDate = location.SnapshotDate.Value;

            Snapshot snapshot = snapshotRepository.GetByExactDateTime(location.PotName, searchedDate);

            if (snapshot == null && searchedDate.TimeOfDay == TimeSpan.Zero)
            {
                List<Snapshot> snapshots = snapshotRepository.GetByDate(location.PotName, searchedDate)
                    .ToList();

                if (snapshots.Count == 1)
                    snapshot = snapshots[0];
                else if (snapshots.Count > 1)
                    throw new Exception($"There are multiple snapshots that match the specified date. Pot = {location.PotName}; Date = {searchedDate}");
            }

            return snapshot;
        }

        return snapshotRepository.GetLast(location.PotName);
    }
}