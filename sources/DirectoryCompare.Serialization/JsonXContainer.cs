// DirectoryCompare
// Copyright (C) 2017 Dust in the Wind
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

namespace DustInTheWind.DirectoryCompare.Serialization
{
    internal class JsonXContainer
    {
        [JsonProperty("original-path")]
        public string OriginalPath { get; set; }

        [JsonProperty("creation-time")]
        public DateTime CreationTime { get; set; }

        [JsonProperty("d")]
        public List<JsonXDirectory> Directories { get; set; }

        [JsonProperty("f")]
        public List<JsonXFile> Files { get; set; }

        public JsonXContainer()
        {
        }

        public JsonXContainer(XContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            OriginalPath = container.OriginalPath;
            CreationTime = container.CreationTime;

            if (container.Directories == null)
                Directories = null;
            else
                Directories = container.Directories
                    .Select(x => new JsonXDirectory(x))
                    .ToList();

            if (container.Files == null)
                Files = null;
            else
                Files = container.Files
                    .Select(x => new JsonXFile(x))
                    .ToList();
        }

        public XContainer ToContainer()
        {
            return new XContainer
            {
                OriginalPath = OriginalPath,
                CreationTime = CreationTime,
                Directories = GetXDirectories(),
                Files = GetXFiles()
            };
        }

        private List<XDirectory> GetXDirectories()
        {
            if (Directories == null)
                return null;

            return Directories
                .Select(x => x.ToXDirectory())
                .ToList();
        }

        private List<XFile> GetXFiles()
        {
            if (Files == null)
                return null;

            return Files
                .Select(x => x.ToXFile())
                .ToList();
        }
    }
}