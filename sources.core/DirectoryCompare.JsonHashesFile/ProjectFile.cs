﻿// DirectoryCompare
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
using System.IO;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Domain.Utils;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.JsonHashesFile
{
    public class ProjectFile
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonIgnore]
        public DiskPath Path { get; set; }

        public static ProjectFile Create(Project project)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            return new ProjectFile
            {
                Name = project.Name,
                Path = project.Path + "project.json"
            };
        }

        public void Save()
        {
            string json = JsonConvert.SerializeObject(this);

            File.WriteAllText(Path, json);
        }
    }
}