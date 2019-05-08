﻿// DirectoryCompare
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
using DustInTheWind.DirectoryCompare.Entities;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.JsonHashesFile.JsonExport
{
    internal sealed class JsonSnapshot : JsonDirectory
    {
        public Guid Id { get; set; }

        public string OriginalPath { get; set; }

        public DateTime CreationTime { get; set; }

        public JsonSnapshot(JsonTextWriter jsonTextWriter)
            : base(jsonTextWriter)
        {
        }

        public void WriteStart()
        {
            Writer.WriteStartObject();

            Writer.WritePropertyName("serializer");
            Writer.WriteStartObject();
            Writer.WritePropertyName("id");
            Writer.WriteValue(Id);
            Writer.WriteEndObject();

            Writer.WritePropertyName("original-path");
            Writer.WriteValue(OriginalPath);

            Writer.WritePropertyName("creation-time");
            Writer.WriteValue(CreationTime);
        }

        protected override void WriteStartDirectoryInternal(HDirectory directory)
        {
        }
    }
}