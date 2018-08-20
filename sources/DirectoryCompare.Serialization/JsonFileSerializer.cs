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

using System.Diagnostics;
using System.IO;
using DustInTheWind.DirectoryCompare.JsonSerialization;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.Serialization
{
    public class JsonFileSerializer : ISerializer
    {
        public void WriteToFile(XContainer container, string destinationFilePath)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            JsonXContainer jsinXContainer = new JsonXContainer(container);

            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };
            string json = JsonConvert.SerializeObject(jsinXContainer, jsonSerializerSettings);
            File.WriteAllText(destinationFilePath, json);

            stopwatch.Stop();
        }

        public XContainer ReadFromFile(string sourceFilePath)
        {
            string json = File.ReadAllText(sourceFilePath);
            JsonXContainer jsonXContainer = JsonConvert.DeserializeObject<JsonXContainer>(json);

            return jsonXContainer.ToContainer();
        }
    }
}