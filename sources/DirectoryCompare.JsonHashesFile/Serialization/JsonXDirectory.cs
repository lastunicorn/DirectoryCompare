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
using System.Linq;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.JsonHashesFile.Serialization
{
    internal class JsonXDirectory
    {
        [JsonProperty("n")]
        public string Name { get; set; }

        [JsonProperty("d")]
        public List<JsonXDirectory> Directories { get; set; }

        [JsonProperty("f")]
        public List<JsonXFile> Files { get; set; }

        public JsonXDirectory()
        {
        }

        public JsonXDirectory(HDirectory xDirectory)
        {
            if (xDirectory == null) throw new ArgumentNullException(nameof(xDirectory));

            Name = xDirectory.Name;

            if (xDirectory.Directories == null)
                Directories = null;
            else
                Directories = xDirectory.Directories
                    .Select(x => new JsonXDirectory(x))
                    .ToList();

            if (xDirectory.Files == null)
                Files = null;
            else
                Files = xDirectory.Files
                    .Select(x => new JsonXFile(x))
                    .ToList();
        }

        public HDirectory ToHDirectory()
        {
            HDirectory directory = new HDirectory
            {
                Name = Name
            };

            List<HDirectory> directories = GetHDirectories();
            if (directories != null)
                directory.Directories.AddRange(directories);

            List<HFile> files = GetHFiles();
            if (files != null)
                directory.Files = files;

            return directory;
        }

        private List<HDirectory> GetHDirectories()
        {
            return Directories?
                .Select(x => x.ToHDirectory())
                .ToList();
        }

        private List<HFile> GetHFiles()
        {
            return Files?
                .Select(x => x.ToHFile())
                .ToList();
        }
    }
}