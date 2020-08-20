// DirectoryCompare
// Copyright (C) 2017-2019 Dust in the Wind
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

using System;
using System.Collections.Generic;
using System.Linq;
using DustInTheWind.DirectoryCompare.Domain.DataAccess;
using DustInTheWind.DirectoryCompare.Domain.Entities;

namespace DustInTheWind.DirectoryCompare.Domain.Comparison
{
    public static class SnapshotRepositoryExtensions
    {
        public static IEnumerable<HFile> EnumerateFiles(this ISnapshotRepository snapshotRepository, SnapshotLocation snapshotLocation, BlackList blackList = null)
        {
            Snapshot snapshot = snapshotRepository.GetSnapshot(snapshotLocation);

            return snapshot == null
                ? Enumerable.Empty<HFile>()
                : snapshot.EnumerateFiles(snapshotLocation.InternalPath, blackList);
        }

        private static Snapshot GetSnapshot(this ISnapshotRepository snapshotRepository, SnapshotLocation snapshotLocation)
        {
            if (string.IsNullOrEmpty(snapshotLocation.PotName))
                return null;

            if (snapshotLocation.SnapshotIndex.HasValue)
                return snapshotRepository.GetByIndex(snapshotLocation.PotName, snapshotLocation.SnapshotIndex.Value);

            if (!snapshotLocation.SnapshotDate.HasValue)
                return snapshotRepository.GetLast(snapshotLocation.PotName);

            DateTime searchedDate = snapshotLocation.SnapshotDate.Value;

            Snapshot snapshot = snapshotRepository.GetByExactDateTime(snapshotLocation.PotName, searchedDate);

            if (snapshot == null && searchedDate.TimeOfDay == TimeSpan.Zero)
            {
                List<Snapshot> snapshots = snapshotRepository.GetByDate(snapshotLocation.PotName, searchedDate)
                    .ToList();

                if (snapshots.Count == 1)
                    snapshot = snapshots[0];
                else if (snapshots.Count > 1)
                    throw new Exception($"There are multiple snapshots that match the specified date. Pot = {snapshotLocation.PotName}; Date = {searchedDate}");
            }

            return snapshot;
        }
    }
}