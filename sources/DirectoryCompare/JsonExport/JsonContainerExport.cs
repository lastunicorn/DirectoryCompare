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
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.JsonExport
{
    internal class JsonContainerExport : JsonDirectoryExport
    {
        public Guid Id { get; set; }
        public string OriginalPath { get; set; }

        public JsonContainerExport(JsonTextWriter jsonTextWriter)
            :base(jsonTextWriter)
        {
        }

        protected override void DoOpen(XDirectory xDirectory)
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
            Writer.WriteValue(DateTime.UtcNow);
        }
    }
}