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
    internal class JsonSnapshot
    {
        [JsonProperty("serializer")]
        public SerializerInfo SerializerInfo { get; set; }

        [JsonProperty("original-path")]
        public string OriginalPath { get; set; }

        [JsonProperty("creation-time")]
        public DateTime CreationTime { get; set; }

        [JsonProperty("d")]
        public List<JsonDirectory> Directories { get; set; }

        [JsonProperty("f")]
        public List<JsonFile> Files { get; set; }

        public JsonSnapshot()
        {
        }

        public JsonSnapshot(Snapshot snapshot)
        {
            if (snapshot == null) throw new ArgumentNullException(nameof(snapshot));

            OriginalPath = snapshot.OriginalPath;
            CreationTime = snapshot.CreationTime;

            if (snapshot.Directories == null)
                Directories = null;
            else
                Directories = snapshot.Directories
                    .Select(x => new JsonDirectory(x))
                    .ToList();

            if (snapshot.Files == null)
                Files = null;
            else
                Files = snapshot.Files
                    .Select(x => new JsonFile(x))
                    .ToList();
        }

        public Snapshot ToSnapshot()
        {
            Snapshot snapshot = new Snapshot
            {
                OriginalPath = OriginalPath,
                CreationTime = CreationTime
            };

            IEnumerable<HDirectory> newDirectories = GetHDirectories();
            snapshot.Directories.AddRange(newDirectories);

            IEnumerable<HFile> newFiles = GetHFiles();
            snapshot.Files.AddRange(newFiles);

            return snapshot;
        }

        private IEnumerable<HDirectory> GetHDirectories()
        {
            if (Directories == null)
                return null;

            return Directories
                .Select(x => x.ToHDirectory())
                .ToList();
        }

        private IEnumerable<HFile> GetHFiles()
        {
            if (Files == null)
                return null;

            return Files
                .Select(x => x.ToHFile())
                .ToList();
        }
    }
}