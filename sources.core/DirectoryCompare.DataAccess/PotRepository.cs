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

using DustInTheWind.DirectoryCompare.Domain;
using DustInTheWind.DirectoryCompare.Domain.DataAccess;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DustInTheWind.DirectoryCompare.DataAccess
{
    public class PotRepository : IPotRepository
    {
        public List<Pot> Get()
        {
            return GetAllPotInfo()
                .Select(x => new Pot
                {
                    Name = x.Name,
                    Path = x.Path
                })
                .ToList();
        }

        public Pot Get(string name)
        {
            return GetAllPotInfo()
                .Where(x => x.Name == name)
                .Select(x => new Pot
                {
                    Name = x.Name,
                    Path = x.Path
                })
                .FirstOrDefault();
        }

        private static IEnumerable<JPotInfo> GetAllPotInfo()
        {
            return Directory.GetDirectories(".")
                .Select(x => Path.Combine(x, "info.json"))
                .Where(File.Exists)
                .Select(File.ReadAllText)
                .Select(x =>
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<JPotInfo>(x);
                    }
                    catch
                    {
                        return null;
                    }
                })
                .Where(x => x != null);
        }

        public void Add(Pot pot)
        {
            string directoryName = CreatePotDirectory();
            CreateInfoFile(pot, directoryName);
        }

        private static string CreatePotDirectory()
        {
            for (int i = 0; i < 10000; i++)
            {
                Guid guid = Guid.NewGuid();
                string path = guid.ToString("D");

                if (Directory.Exists(path))
                    continue;

                Directory.CreateDirectory(path);
                return path;
            }

            throw new Exception("Could not find a valid name for the pot's directory. All the tried name already exist.");
        }

        private static void CreateInfoFile(Pot pot, string directoryName)
        {
            JPotInfo jPotInfo = new JPotInfo
            {
                Name = pot.Name,
                Path = pot.Path
            };

            string json = JsonConvert.SerializeObject(jPotInfo);

            string infoFilePath = Path.Combine(directoryName, "info.json");
            File.WriteAllText(infoFilePath, json);
        }
    }
}