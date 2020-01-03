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

using DustInTheWind.DirectoryCompare.Domain.DataAccess;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DustInTheWind.DirectoryCompare.Domain.Comparison
{
    public class FileDuplicates : IEnumerable<FileDuplicate>
    {
        private readonly ISnapshotRepository snapshotRepository;

        public SnapshotLocation SnapshotLeft { get; set; }
        public SnapshotLocation SnapshotRight { get; set; }
        public bool CheckFilesExist { get; set; }

        public FileDuplicates(ISnapshotRepository snapshotRepository)
        {
            this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
        }

        public IEnumerator<FileDuplicate> GetEnumerator()
        {
            List<HFile> filesLeft = GetFiles(SnapshotLeft);

            if (filesLeft == null)
                yield break;

            List<HFile> filesRight = GetFiles(SnapshotRight);

            if (filesRight == null)
            {
                for (int i = 0; i < filesLeft.Count; i++)
                {
                    HFile fileLeft = filesLeft[i];

                    for (int j = i + 1; j < filesLeft.Count; j++)
                    {
                        HFile fileRight = filesLeft[j];

                        FileDuplicate fileDuplicate = new FileDuplicate(fileLeft, fileRight, CheckFilesExist);

                        if (fileDuplicate.AreEqual)
                            yield return fileDuplicate;
                    }
                }
            }
            else
            {
                foreach (HFile fileLeft in filesLeft)
                {
                    foreach (HFile fileRight in filesRight)
                    {
                        FileDuplicate fileDuplicate = new FileDuplicate(fileLeft, fileRight, CheckFilesExist);

                        if (fileDuplicate.AreEqual)
                            yield return fileDuplicate;
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private List<HFile> GetFiles(SnapshotLocation snapshotLocation)
        {
            Snapshot snapshot;

            if (string.IsNullOrEmpty(snapshotLocation.PotName))
                return null;

            if (snapshotLocation.SnapshotIndex.HasValue)
            {
                snapshot = snapshotRepository.GetByIndex(snapshotLocation.PotName, snapshotLocation.SnapshotIndex.Value);
            }
            else if (snapshotLocation.SnapshotDate.HasValue)
            {
                DateTime searchedDate = snapshotLocation.SnapshotDate.Value;

                snapshot = snapshotRepository.GetByExactDateTime(snapshotLocation.PotName, searchedDate);

                if (snapshot == null && searchedDate.TimeOfDay == TimeSpan.Zero)
                {
                    List<Snapshot> snapshots = snapshotRepository.GetByDate(snapshotLocation.PotName, searchedDate)
                        .ToList();

                    if (snapshots.Count == 1)
                        snapshot = snapshots[0];
                    else if (snapshots.Count > 1)
                        throw new Exception($"There are multiple snapshots that match the specified date. Pot = {snapshotLocation.PotName}; Date = {searchedDate}");
                }
            }
            else
            {
                snapshot = snapshotRepository.GetLast(snapshotLocation.PotName);
            }

            if (snapshot == null)
                return null;

            IEnumerable<HFile> filesQuery = snapshot.EnumerateFiles();

            if (snapshotLocation.InternalPath != null)
                filesQuery = filesQuery.Where(x => x.GetPath().StartsWith(snapshotLocation.InternalPath));

            return filesQuery.ToList();
        }
    }
}