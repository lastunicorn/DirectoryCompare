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
using DustInTheWind.DirectoryCompare.Entities;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.JsonHashesFile.Serialization
{
    internal class JsonXContainer
    {
        [JsonProperty("serializer")]
        public SerializerInfo SerializerInfo { get; set; }

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

        public JsonXContainer(HContainer container)
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

        public HContainer ToContainer()
        {
            return new HContainer
            {
                OriginalPath = OriginalPath,
                CreationTime = CreationTime,
                Directories = GetHDirectories(),
                Files = GetHFiles()
            };
        }

        private List<HDirectory> GetHDirectories()
        {
            if (Directories == null)
                return null;

            return Directories
                .Select(x => x.ToHDirectory())
                .ToList();
        }

        private List<HFile> GetHFiles()
        {
            if (Files == null)
                return null;

            return Files
                .Select(x => x.ToHFile())
                .ToList();
        }
    }
}