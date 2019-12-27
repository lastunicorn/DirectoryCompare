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
using DustInTheWind.DirectoryCompare.Domain.Utils;
using Newtonsoft.Json;
using System;
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
            
            string blackListFilePath = Path.Combine(potDirectoryPath, "bl");

            if (!File.Exists(blackListFilePath))
                return new PathCollection();

            return ReadBlackList(blackListFilePath);
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

        private static PathCollection ReadBlackList(string filePath)
        {
            Console.WriteLine("Reading black list from file: {0}", filePath);

            string[] list = File.Exists(filePath)
                ? File.ReadAllLines(filePath)
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Where(x => !x.StartsWith("#"))
                    .ToArray()
                : new string[0];

            return new PathCollection(list);
        }
    }
}