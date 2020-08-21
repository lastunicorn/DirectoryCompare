// DirectoryCompare
// Copyright (C) 2017-2020 Dust in the Wind
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
using DustInTheWind.DirectoryCompare.Domain;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.JsonHashesFile.JsonExport
{
    internal sealed class JsonSnapshotReader
    {
        private readonly JsonTextReader jsonTextReader;
        private readonly SnapshotBuilder snapshotBuilder;

        public Guid Id => new Guid("9E93055D-7BDE-4F55-B340-DD5A4880D96E");

        public JsonSnapshotReader(JsonTextReader jsonTextReader, SnapshotBuilder snapshotBuilder)
        {
            this.jsonTextReader = jsonTextReader ?? throw new ArgumentNullException(nameof(jsonTextReader));
            this.snapshotBuilder = snapshotBuilder ?? throw new ArgumentNullException(nameof(snapshotBuilder));
        }

        public void Read()
        {
            ReadStartObject();

            while (true)
            {
                string propertyName = ReadNextPropertyName();

                switch (propertyName)
                {
                    case "serializer-id":
                        Guid id = ReadGuidValue();

                        if (id != Id)
                        {
                            // Warning !!! The json file was serialized with a different serializer. It may not be compatible with the current deserializer.
                        }

                        break;

                    case "original-path":
                        string originalPath = ReadStringValue();
                        snapshotBuilder.SetOriginalPath(originalPath);
                        break;

                    case "creation-time":
                        DateTime creationTime = ReadDateValue();
                        snapshotBuilder.SetCreationTime(creationTime);
                        break;

                    case null:
                        return;
                }
            }
        }

        private string ReadNextPropertyName()
        {
            while (true)
            {
                if (!jsonTextReader.Read())
                    throw new Exception("Property cannot be read. There is no more data to read.");

                switch (jsonTextReader.TokenType)
                {
                    case JsonToken.None:
                    case JsonToken.StartObject:
                    case JsonToken.StartArray:
                    case JsonToken.StartConstructor:
                    case JsonToken.Comment:
                    case JsonToken.Raw:
                    case JsonToken.Integer:
                    case JsonToken.Float:
                    case JsonToken.String:
                    case JsonToken.Boolean:
                    case JsonToken.Null:
                    case JsonToken.Undefined:
                    case JsonToken.EndArray:
                    case JsonToken.EndConstructor:
                    case JsonToken.Date:
                    case JsonToken.Bytes:
                        break;

                    case JsonToken.EndObject:
                        return null;

                    case JsonToken.PropertyName:
                        return jsonTextReader.Value as string;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private string ReadStringValue()
        {
            if (!jsonTextReader.Read())
                throw new Exception("Property cannot be read. There is no more data to read.");

            if (jsonTextReader.TokenType != JsonToken.String)
                throw new Exception("The read token is not a string.");

            return jsonTextReader.Value as string;
        }

        private DateTime ReadDateValue()
        {
            if (!jsonTextReader.Read())
                throw new Exception("Property cannot be read. There is no more data to read.");

            if (jsonTextReader.TokenType != JsonToken.Date)
                throw new Exception("The read token is not a date.");

            return (DateTime)jsonTextReader.Value;
        }

        private Guid ReadGuidValue()
        {
            if (!jsonTextReader.Read())
                throw new Exception("Property cannot be read. There is no more data to read.");

            if (jsonTextReader.TokenType != JsonToken.String)
                throw new Exception("The read token is not a guid.");

            if (!(jsonTextReader.Value is string value))
                throw new Exception("The read token is not a guid.");

            try
            {
                return new Guid(value);
            }
            catch (Exception ex)
            {
                throw new Exception("The read token is not a guid.", ex);
            }
        }

        private void ReadStartObject()
        {
            if (!jsonTextReader.Read())
                throw new Exception("Object cannot be read. There is no more data to read.");

            if (jsonTextReader.TokenType != JsonToken.StartObject)
                throw new Exception("The read token is not an object.");
        }
    }
}