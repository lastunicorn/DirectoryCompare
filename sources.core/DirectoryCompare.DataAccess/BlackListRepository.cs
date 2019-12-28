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
using DustInTheWind.DirectoryCompare.Domain.DataAccess;
using DustInTheWind.DirectoryCompare.Domain.Utils;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace DustInTheWind.DirectoryCompare.DataAccess
{
    public class BlackListRepository : IBlackListRepository
    {
        public PathCollection Get(string potName)
        {
            string potDirectoryPath = Directory.GetDirectories(".")
                .FirstOrDefault(x => IsPot(x, potName));

            if (potDirectoryPath == null)
                throw new Exception($"There is no pot with name '{potName}'.");

            string blackListFilePath = Path.Combine(potDirectoryPath, "bl");

            if (!File.Exists(blackListFilePath))
                return new PathCollection();

            return ReadBlackList(blackListFilePath);
        }

        public void Add(string potName, DiskPath path)
        {
            string potDirectoryPath = Directory.GetDirectories(".")
                .FirstOrDefault(x => IsPot(x, potName));

            if (potDirectoryPath == null)
                throw new Exception($"There is no pot with name '{potName}'.");

            string blackListFilePath = Path.Combine(potDirectoryPath, "bl");

            PathCollection blackList = File.Exists(blackListFilePath)
                ? ReadBlackList(blackListFilePath)
                : new PathCollection();

            if (!blackList.Contains(path))
            {
                blackList.Add(path);
                WriteBlackList(blackListFilePath, blackList);
            }
        }

        public void Delete(string potName, DiskPath path)
        {
            string potDirectoryPath = Directory.GetDirectories(".")
                .FirstOrDefault(x => IsPot(x, potName));

            if (potDirectoryPath == null)
                throw new Exception($"There is no pot with name '{potName}'.");

            string blackListFilePath = Path.Combine(potDirectoryPath, "bl");

            PathCollection blackList = File.Exists(blackListFilePath)
                ? ReadBlackList(blackListFilePath)
                : new PathCollection();

            blackList.Remove(path);
            WriteBlackList(blackListFilePath, blackList);
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

        private static PathCollection ReadBlackList(string filePath)
        {
            List<string> list = File.Exists(filePath)
                ? File.ReadAllLines(filePath)
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Where(x => !x.StartsWith("#"))
                    .ToList()
                : new List<string>();

            return new PathCollection(list);
        }

        private static void WriteBlackList(string filePath, PathCollection blackList)
        {
            string backupFilePath = filePath + ".bak";

            if (File.Exists(backupFilePath))
                File.Delete(backupFilePath);

            if (File.Exists(filePath))
                File.Move(filePath, backupFilePath);

            string[] lines = blackList
                .ToArray();

            File.WriteAllLines(filePath, lines);

            if (File.Exists(backupFilePath))
                File.Delete(backupFilePath);
        }
    }
}