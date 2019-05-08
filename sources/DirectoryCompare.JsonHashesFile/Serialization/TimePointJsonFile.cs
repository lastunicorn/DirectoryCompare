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
using System.Diagnostics;
using System.IO;
using DustInTheWind.DirectoryCompare.Entities;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.JsonHashesFile.Serialization
{
    public class TimePointJsonFile
    {
        public Guid Id => new Guid("9E93055D-7BDE-4F55-B340-DD5A4880D96E");

        public HContainer Container { get; private set; }

        public void Save(string destinationFilePath)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            JsonXContainer jsonXContainer = new JsonXContainer(Container)
            {
                SerializerInfo = new SerializerInfo
                {
                    Id = Id
                }
            };

            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };

            string json = JsonConvert.SerializeObject(jsonXContainer, jsonSerializerSettings);
            File.WriteAllText(destinationFilePath, json);

            stopwatch.Stop();
        }

        public static TimePointJsonFile Load(string sourceFilePath)
        {
            using (StreamReader streamReader = File.OpenText(sourceFilePath))
            using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
            {
                JsonSerializer serializer = new JsonSerializer();
                JsonXContainer jsonXContainer = (JsonXContainer)serializer.Deserialize(jsonTextReader, typeof(JsonXContainer));

                return new TimePointJsonFile
                {
                    Container = jsonXContainer.ToContainer()
                };
            }
        }

        /// <summary>
        /// Searches for the first property. It must be "ver" and contain the text "1.0".
        /// </summary>
        public bool CanDeserialize(string sourceFilePath)
        {
            using (StreamReader streamReader = File.OpenText(sourceFilePath))
            using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
            {
                if (!jsonTextReader.Read())
                    return false;

                if (jsonTextReader.TokenType != JsonToken.StartObject)
                    return false;

                if (!jsonTextReader.Read())
                    return false;

                if (jsonTextReader.TokenType != JsonToken.PropertyName)
                    return false;

                if (jsonTextReader.Value as string != "serializer")
                    return false;

                if (jsonTextReader.TokenType != JsonToken.StartObject)
                    return false;

                if (!jsonTextReader.Read())
                    return false;

                if (jsonTextReader.TokenType != JsonToken.PropertyName)
                    return false;

                if (jsonTextReader.Value as string != "id")
                    return false;

                if (!jsonTextReader.Read())
                    return false;

                if (jsonTextReader.TokenType != JsonToken.String)
                    return false;

                if (jsonTextReader.ValueType != typeof(string))
                    return false;

                if (!(jsonTextReader.Value is string value))
                    return false;

                try
                {
                    return new Guid(value) == Id;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}