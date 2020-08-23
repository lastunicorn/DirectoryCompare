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
using DustInTheWind.DirectoryCompare.JsonHashesFile;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.DataAccess
{
    internal class PotDirectory
    {
        private const string SnapshotsDirectoryName = "snapshots";

        public string FullPath { get; }

        public bool Exists => FullPath != null && Directory.Exists(FullPath);

        public PotDirectory(string potName)
        {
            FullPath = Directory.GetDirectories(".")
                .FirstOrDefault(x => IsPot(x, potName));
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

        public BlackListFile OpenBlackListFile(string blackListName)
        {
            string blackListPath = Path.Combine(FullPath, blackListName);
            BlackListFile blackListFile = new BlackListFile(blackListPath);
            blackListFile.Open();
            return blackListFile;
        }

        public IEnumerable<SnapshotFile> GetSnapshotFiles()
        {
            string snapshotsDirectoryPath = Path.Combine(FullPath, SnapshotsDirectoryName);

            if (!Directory.Exists(snapshotsDirectoryPath))
                return Enumerable.Empty<SnapshotFile>();

            return Directory.GetFiles(snapshotsDirectoryPath)
                .Select(x => new SnapshotFile(x))
                .OrderByDescending(x => x.CreationTime);
        }

        public SnapshotFile CreateSnapshotFile(in DateTime creationTime)
        {
            string snapshotFileName = string.Format("{0:yyyy MM dd HHmmss}.json", creationTime);
            string snapshotsDirectoryPath = Path.Combine(FullPath, SnapshotsDirectoryName);
            string snapshotFilePath = Path.Combine(snapshotsDirectoryPath, snapshotFileName);

            return new SnapshotFile(snapshotFilePath);
        }
    }
}