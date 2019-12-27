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
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using DustInTheWind.DirectoryCompare.JsonHashesFile.Serialization;

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

        public Snapshot GetLast(string potName)
        {
            string snapshotsDirectoryPath = GetSnapshotsDirectoryPath(potName);
            string[] snapshotFileNames = Directory.GetFiles(snapshotsDirectoryPath);

            string snapshotFileName = snapshotFileNames
                .OrderByDescending(x => x)
                .FirstOrDefault();

            if (snapshotFileName == null)
                return null;

            SnapshotJsonFile file = SnapshotJsonFile.Load(snapshotFileName);
            return file.Snapshot;
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
                JPotInfo jPotInfo = JsonConvert.DeserializeObject<JPotInfo>(json);
                return jPotInfo.Name == potName;
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