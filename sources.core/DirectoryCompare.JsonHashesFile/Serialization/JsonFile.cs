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
using DustInTheWind.DirectoryCompare.Domain.Entities;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.JsonHashesFile.Serialization
{
    internal class JsonFile
    {
        [JsonProperty("n")]
        public string Name { get; set; }

        [JsonProperty("s")]
        public ulong Size { get; set; }

        [JsonProperty("m")]
        public DateTime LastModifiedTime { get; set; }

        [JsonProperty("h")]
        public byte[] Hash { get; set; }

        public JsonFile()
        {
        }

        public JsonFile(HFile file)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            Name = file.Name;
            Size = file.Size;
            Hash = file.Hash;
            LastModifiedTime = file.LastModifiedTime;
        }

        public HFile ToHFile()
        {
            return new HFile
            {
                Name = Name,
                Size = Size,
                Hash = Hash,
                LastModifiedTime = LastModifiedTime
            };
        }
    }
}