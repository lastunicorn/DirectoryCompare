// DirectoryCompare
// Copyright (C) 2017-2020 Dust in the Wind
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
using System.IO;
using System.Linq;
using DustInTheWind.DirectoryCompare.Domain.DataAccess;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.JsonHashesFile;

namespace DustInTheWind.DirectoryCompare.DataAccess
{
    public class SnapshotRepository : ISnapshotRepository
    {
        public Stream CreateStream(string potName)
        {
            PotDirectory potDirectory = new PotDirectory(potName);

            if (!potDirectory.Exists)
                throw new Exception($"There is no pot with name '{potName}'.");

            SnapshotFile snapshotFile = potDirectory.CreateSnapshotFile(DateTime.UtcNow);
            return snapshotFile.OpenStream();
        }

        public IEnumerable<Snapshot> GetByPot(string potName)
        {
            PotDirectory potDirectory = new PotDirectory(potName);

            if (!potDirectory.Exists)
                throw new Exception($"There is no pot with name '{potName}'.");

            IEnumerable<SnapshotFile> allSnapshotFiles = potDirectory.GetSnapshotFiles();

            foreach (SnapshotFile snapshotFile in allSnapshotFiles)
            {
                snapshotFile.Open();
                yield return snapshotFile.Snapshot;
            }
        }

        public Snapshot GetByIndex(string potName, int index = 0)
        {
            PotDirectory potDirectory = new PotDirectory(potName);

            if (!potDirectory.Exists)
                throw new Exception($"There is no pot with name '{potName}'.");

            SnapshotFile snapshotFile = potDirectory.GetSnapshotFiles()
                .Skip(index)
                .FirstOrDefault();

            if (snapshotFile == null)
                return null;

            snapshotFile.Open();
            return snapshotFile.Snapshot;
        }

        public Snapshot GetLast(string potName)
        {
            return GetByIndex(potName);
        }

        public IEnumerable<Snapshot> GetByDate(string potName, DateTime dateTime)
        {
            PotDirectory potDirectory = new PotDirectory(potName);

            if (!potDirectory.Exists)
                throw new Exception($"There is no pot with name '{potName}'.");

            IEnumerable<SnapshotFile> snapshotFiles = potDirectory.GetSnapshotFiles()
                .Where(x => x.CreationTime.HasValue && x.CreationTime.Value.Date == dateTime.Date);

            foreach (SnapshotFile snapshotFile in snapshotFiles)
            {
                snapshotFile.Open();
                yield return snapshotFile.Snapshot;
            }
        }

        public Snapshot GetByExactDateTime(string potName, DateTime dateTime)
        {
            PotDirectory potDirectory = new PotDirectory(potName);

            if (!potDirectory.Exists)
                throw new Exception($"There is no pot with name '{potName}'.");

            SnapshotFile snapshotFile = potDirectory.GetSnapshotFiles()
                .FirstOrDefault(x => x.CreationTime.HasValue && x.CreationTime.Value == dateTime);

            if (snapshotFile == null)
                return null;

            snapshotFile.Open();
            return snapshotFile.Snapshot;
        }

        public void Add(string potName, Snapshot snapshot)
        {
            PotDirectory potDirectory = new PotDirectory(potName);

            if (!potDirectory.Exists)
                throw new Exception($"There is no pot with name '{potName}'.");

            SnapshotFile snapshotFile = potDirectory.CreateSnapshotFile(snapshot.CreationTime);
            snapshotFile.Open();
            snapshotFile.Snapshot = snapshot;
            snapshotFile.Save();
        }

        public void DeleteByIndex(string potName, int index = 0)
        {
            PotDirectory potDirectory = new PotDirectory(potName);

            if (!potDirectory.Exists)
                throw new Exception($"There is no pot with name '{potName}'.");

            SnapshotFile snapshotFile = potDirectory.GetSnapshotFiles()
                .Skip(index)
                .FirstOrDefault();

            snapshotFile?.Delete();
        }

        public void DeleteLast(string potName)
        {
            DeleteByIndex(potName);
        }

        public bool DeleteSingleByDate(string potName, DateTime dateTime)
        {
            PotDirectory potDirectory = new PotDirectory(potName);

            if (!potDirectory.Exists)
                throw new Exception($"There is no pot with name '{potName}'.");

            SnapshotFile[] snapshotFiles = potDirectory.GetSnapshotFiles()
                .Where(x => x.CreationTime.HasValue && x.CreationTime.Value.Date == dateTime.Date)
                .ToArray();

            if (snapshotFiles.Length == 0)
                return false;

            if (snapshotFiles.Length > 1)
                throw new Exception($"There are multiple snapshots that match the specified date. Pot = {potName}; Date = {dateTime}");

            snapshotFiles.First().Delete();
            return true;
        }

        public bool DeleteByExactDateTime(string potName, DateTime dateTime)
        {
            PotDirectory potDirectory = new PotDirectory(potName);

            if (!potDirectory.Exists)
                throw new Exception($"There is no pot with name '{potName}'.");

            SnapshotFile snapshotFile = potDirectory.GetSnapshotFiles()
                .FirstOrDefault(x => x.CreationTime.HasValue && x.CreationTime.Value == dateTime);

            if (snapshotFile == null)
                return false;

            snapshotFile.Delete();
            return true;
        }
    }
}