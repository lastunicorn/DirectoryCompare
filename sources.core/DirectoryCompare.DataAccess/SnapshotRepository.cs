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

using DustInTheWind.DirectoryCompare.Domain.DataAccess;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.JsonHashesFile.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DustInTheWind.DirectoryCompare.DataAccess
{
    public class SnapshotRepository : ISnapshotRepository
    {
        private const string SnapshotsDirectoryName = "snapshots";

        public Stream CreateStream(string potName)
        {
            string snapshotsDirectoryPath = GetSnapshotsDirectoryPath(potName);
            EnsureDirectory(snapshotsDirectoryPath);

            string snapshotFileName = string.Format("{0:yyyy MM dd HHmmss}.json", DateTime.UtcNow);
            string snapshotFilePath = Path.Combine(snapshotsDirectoryPath, snapshotFileName);

            return new FileStream(snapshotFilePath, FileMode.CreateNew);
        }

        public IEnumerable<Snapshot> GetByPot(string potName)
        {
            string snapshotsDirectoryPath = GetSnapshotsDirectoryPath(potName);

            if (!Directory.Exists(snapshotsDirectoryPath))
                return Enumerable.Empty<Snapshot>();

            return Directory.GetFiles(snapshotsDirectoryPath)
                .OrderByDescending(x => x)
                .Select(x =>
                {
                    JsonSnapshotFile file = JsonSnapshotFile.Load(x);
                    return file.Snapshot;
                });
        }

        public Snapshot GetByIndex(string potName, int index = 0)
        {
            string snapshotsDirectoryPath = GetSnapshotsDirectoryPath(potName);

            return Directory.GetFiles(snapshotsDirectoryPath)
                .OrderByDescending(x => x)
                .Skip(index)
                .Select(x =>
                {
                    JsonSnapshotFile file = JsonSnapshotFile.Load(x);
                    return file.Snapshot;
                })
                .FirstOrDefault();
        }

        public Snapshot GetLast(string potName)
        {
            return GetByIndex(potName);
        }

        public IEnumerable<Snapshot> GetByDate(string potName, DateTime dateTime)
        {
            string searchedFileName = dateTime.ToString("yyyy MM dd");
            string snapshotsDirectoryPath = GetSnapshotsDirectoryPath(potName);

            return Directory.GetFiles(snapshotsDirectoryPath)
                .Where(x => Path.GetFileName(x).StartsWith(searchedFileName))
                .Select(x =>
                {
                    JsonSnapshotFile file = JsonSnapshotFile.Load(x);
                    return file.Snapshot;
                });
        }

        public Snapshot GetByExactDateTime(string potName, DateTime dateTime)
        {
            string searchedFileName = dateTime.ToString("yyyy MM dd HHmmss") + ".json";
            string snapshotsDirectoryPath = GetSnapshotsDirectoryPath(potName);

            return Directory.GetFiles(snapshotsDirectoryPath)
                .Where(x => Path.GetFileName(x) == searchedFileName)
                .Select(x =>
                {
                    JsonSnapshotFile file = JsonSnapshotFile.Load(x);
                    return file.Snapshot;
                })
                .FirstOrDefault();
        }

        public void Add(string potName, Snapshot snapshot)
        {
            string snapshotsDirectoryPath = GetSnapshotsDirectoryPath(potName);
            EnsureDirectory(snapshotsDirectoryPath);

            string snapshotFileName = string.Format("{0:yyyy MM dd HHmmss}.json", snapshot.CreationTime);
            string snapshotFilePath = Path.Combine(snapshotsDirectoryPath, snapshotFileName);

            JsonSnapshotFile jsonSnapshotFile = new JsonSnapshotFile
            {
                Snapshot = snapshot
            };

            jsonSnapshotFile.Save(snapshotFilePath);
        }

        public void DeleteByIndex(string potName, int index = 0)
        {
            string snapshotsDirectoryPath = GetSnapshotsDirectoryPath(potName);

            string snapshotFileName = Directory.GetFiles(snapshotsDirectoryPath)
                .OrderByDescending(x => x)
                .Skip(index)
                .FirstOrDefault();

            if (snapshotFileName != null)
                File.Delete(snapshotFileName);
        }

        public void DeleteLast(string potName)
        {
            DeleteByIndex(potName);
        }

        public bool DeleteSingleByDate(string potName, DateTime dateTime)
        {
            string searchedFileName = dateTime.ToString("yyyy MM dd");
            string snapshotsDirectoryPath = GetSnapshotsDirectoryPath(potName);

            List<string> snapshotFileNames = Directory.GetFiles(snapshotsDirectoryPath)
                .Where(x => Path.GetFileName(x).StartsWith(searchedFileName))
                .ToList();

            if (snapshotFileNames.Count == 0)
                return false;

            if (snapshotFileNames.Count > 1)
                throw new Exception($"There are multiple snapshots that match the specified date. Pot = {potName}; Date = {dateTime}");

            File.Delete(snapshotFileNames.First());
            return true;
        }

        public bool DeleteByExactDateTime(string potName, DateTime dateTime)
        {
            string searchedFileName = dateTime.ToString("yyyy MM dd HHmmss") + ".json";
            string snapshotsDirectoryPath = GetSnapshotsDirectoryPath(potName);

            string snapshotFileName = Directory.GetFiles(snapshotsDirectoryPath)
                .FirstOrDefault(x => Path.GetFileName(x) == searchedFileName);

            if (snapshotFileName == null)
                return false;

            File.Delete(snapshotFileName);
            return true;
        }

        private static string GetSnapshotsDirectoryPath(string potName)
        {
            string potDirectoryPath = GetPotRootPath(potName);
            return Path.Combine(potDirectoryPath, SnapshotsDirectoryName);
        }

        private static string GetPotRootPath(string potName)
        {
            string potDirectoryPath = Directory.GetDirectories(".")
                .FirstOrDefault(x => IsPot(x, potName));

            if (potDirectoryPath == null)
                throw new Exception($"There is no pot with the name: {potName}");

            return potDirectoryPath;
        }

        private static bool IsPot(string directoryPath, string potName)
        {
            try
            {
                string infoFilePath = Path.Combine(directoryPath, "info.json");

                if (!File.Exists(infoFilePath))
                    return false;

                string json = File.ReadAllText(infoFilePath);
                JInfo jInfo = JsonConvert.DeserializeObject<JInfo>(json);
                return jInfo.Name == potName;
            }
            catch
            {
                return false;
            }
        }

        private static void EnsureDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
        }
    }
}