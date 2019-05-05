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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DustInTheWind.DirectoryCompare.Serialization
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

        public JsonXDirectory(XDirectory xDirectory)
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

        public XDirectory ToXDirectory()
        {
            XDirectory directory = new XDirectory
            {
                Name = Name
            };

            List<XDirectory> directories = GetXDirectories();
            if (directories != null)
                directory.Directories.AddRange(directories);

            List<XFile> files = GetXFiles();
            if (files != null)
                directory.Files = files;

            return directory;
        }

        private List<XDirectory> GetXDirectories()
        {
            return Directories?
                .Select(x => x.ToXDirectory())
                .ToList();
        }

        private List<XFile> GetXFiles()
        {
            return Files?
                .Select(x => x.ToXFile())
                .ToList();
        }
    }
}