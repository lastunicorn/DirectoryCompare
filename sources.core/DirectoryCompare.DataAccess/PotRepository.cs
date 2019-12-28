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
            return GetAllPots()
                .ToList();
        }

        public Pot Get(string name)
        {
            return GetAllPots()
                .FirstOrDefault(x => x.Name == name);
        }

        private static IEnumerable<Pot> GetAllPots()
        {
            return Directory.GetDirectories(".")
                .Select(x => new
                {
                    DirectoryName = Path.GetFileName(x),
                    InfoFilePath = Path.Combine(x, "info.json")
                })
                .Where(x=> File.Exists(x.InfoFilePath))
                .Select(x=> new
                {
                    DirectoryName = x.DirectoryName,
                    InfoFileContent = File.ReadAllText(x.InfoFilePath)
                })
                .Select(x =>
                {
                    try
                    {
                        JInfo jInfo = JsonConvert.DeserializeObject<JInfo>(x.InfoFileContent);

                        return new Pot
                        {
                            Guid = new Guid(x.DirectoryName),
                            Name = jInfo.Name,
                            Path = jInfo.Path
                        };
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
            JInfo jInfo = new JInfo
            {
                Name = pot.Name,
                Path = pot.Path
            };

            string json = JsonConvert.SerializeObject(jInfo);

            string infoFilePath = Path.Combine(directoryName, "info.json");
            File.WriteAllText(infoFilePath, json);
        }
    }
}