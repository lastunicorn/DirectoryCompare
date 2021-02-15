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
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.JFiles.SnapshotFileModel
{
    public sealed class JSnapshotReader : JReader
    {
        public JSnapshotFieldType CurrentPropertyType { get; private set; } = JSnapshotFieldType.None;

        public JSnapshotReader(JsonTextReader jsonTextReader)
            : base(jsonTextReader)
        {
        }

        public JSnapshotFieldType MoveToNext()
        {
            switch (state)
            {
                case JReaderState.New:
                    state = JReaderState.InProgress;
                    break;

                case JReaderState.InProgress:
                    break;

                case JReaderState.Finished:
                    throw new Exception("The reader already finished reading the json object.");

                default:
                    throw new Exception("Invalid reader state.");
            }

            try
            {
                bool success = MoveToNextProperty();

                CurrentPropertyType = success
                    ? jsonTextReader.Value switch
                    {
                        "serializer-id" => JSnapshotFieldType.SerializerId,
                        "original-path" => JSnapshotFieldType.OriginalPath,
                        "creation-time" => JSnapshotFieldType.CreationTime,
                        _ => throw new Exception("Invalid field in directory object.")
                    }
                    : JSnapshotFieldType.None;

                return CurrentPropertyType;
            }
            catch
            {
                CurrentPropertyType = JSnapshotFieldType.None;
                throw;
            }
        }

        public Guid ReadSerializerId()
        {
            if (CurrentPropertyType != JSnapshotFieldType.SerializerId)
                throw new Exception("Current property is not the serializer id.");

            string rawValue = jsonTextReader.ReadAsString();

            return Guid.Parse(rawValue);
        }

        public string ReadOriginalPath()
        {
            if (CurrentPropertyType != JSnapshotFieldType.OriginalPath)
                throw new Exception("Current property is not the original path.");

            return jsonTextReader.ReadAsString();
        }

        public DateTime ReadCreationTime()
        {
            if (CurrentPropertyType != JSnapshotFieldType.CreationTime)
                throw new Exception("Current property is not the creation time.");

            DateTime? readAsDateTime = jsonTextReader.ReadAsDateTime();
            return readAsDateTime.Value;
        }

        public IEnumerable<JFileReader> ReadFiles()
        {
            if (CurrentPropertyType != JSnapshotFieldType.FileCollection)
                throw new Exception("Current property is not the file collection.");

            return ReadFilesCollection();
        }

        public IEnumerable<JDirectoryReader> ReadSubDirectories()
        {
            if (CurrentPropertyType != JSnapshotFieldType.DirectoryCollection)
                throw new Exception("Current property is not the sub-directory collection.");

            return ReadDirectoriesCollection();
        }

        //public void Read()
        //{
        //    ReadStartObject();

        //    while (true)
        //    {
        //        string propertyName = ReadNextPropertyName();

        //        switch (propertyName)
        //        {
        //            case "serializer-id":
        //                Guid id = ReadGuidValue();

        //                if (id != Id)
        //                {
        //                    // Warning !!! The json file was serialized with a different serializer. It may not be compatible with the current deserializer.
        //                }

        //                break;

        //            case "original-path":
        //                string originalPath = ReadStringValue();
        //                //snapshotBuilder.SetOriginalPath(originalPath);
        //                break;

        //            case "creation-time":
        //                DateTime creationTime = ReadDateValue();
        //                //snapshotBuilder.SetCreationTime(creationTime);
        //                break;

        //            case "d":
        //                jsonTextReader.Read();
        //                if (jsonTextReader.TokenType == JsonToken.StartArray)
        //                {
        //                    while (jsonTextReader.Read())
        //                    {
        //                        if (jsonTextReader.TokenType == JsonToken.StartObject)
        //                        {
        //                            //JDirectoryReader jDirectoryReader = new JDirectoryReader(jsonTextReader, snapshotBuilder);
        //                            //HDirectory directory = jDirectoryReader.Read();
        //                        }
        //                        else if (jsonTextReader.TokenType == JsonToken.EndArray)
        //                        {
        //                            break;
        //                        }
        //                    }
        //                }

        //                break;

        //            case null:
        //                return;
        //        }
        //    }
        //}

        //private void ReadStartObject()
        //{
        //    if (!jsonTextReader.Read())
        //        throw new Exception("Object cannot be read. There is no more data to read.");

        //    if (jsonTextReader.TokenType != JsonToken.StartObject)
        //        throw new Exception("The read token is not an object.");
        //}

        //private string ReadNextPropertyName()
        //{
        //    while (true)
        //    {
        //        if (!jsonTextReader.Read())
        //            throw new Exception("Property cannot be read. There is no more data to read.");

        //        switch (jsonTextReader.TokenType)
        //        {
        //            case JsonToken.None:
        //            case JsonToken.StartObject:
        //            case JsonToken.StartArray:
        //            case JsonToken.StartConstructor:
        //            case JsonToken.Comment:
        //            case JsonToken.Raw:
        //            case JsonToken.Integer:
        //            case JsonToken.Float:
        //            case JsonToken.String:
        //            case JsonToken.Boolean:
        //            case JsonToken.Null:
        //            case JsonToken.Undefined:
        //            case JsonToken.EndArray:
        //            case JsonToken.EndConstructor:
        //            case JsonToken.Date:
        //            case JsonToken.Bytes:
        //                break;

        //            case JsonToken.EndObject:
        //                return null;

        //            case JsonToken.PropertyName:
        //                return jsonTextReader.Value as string;

        //            default:
        //                throw new ArgumentOutOfRangeException();
        //        }
        //    }
        //}

        //private string ReadStringValue()
        //{
        //    if (!jsonTextReader.Read())
        //        throw new Exception("Property cannot be read. There is no more data to read.");

        //    if (jsonTextReader.TokenType != JsonToken.String)
        //        throw new Exception("The read token is not a string.");

        //    return jsonTextReader.Value as string;
        //}

        //private DateTime ReadDateValue()
        //{
        //    if (!jsonTextReader.Read())
        //        throw new Exception("Property cannot be read. There is no more data to read.");

        //    if (jsonTextReader.TokenType != JsonToken.Date)
        //        throw new Exception("The read token is not a date.");

        //    return (DateTime)jsonTextReader.Value;
        //}

        //private Guid ReadGuidValue()
        //{
        //    if (!jsonTextReader.Read())
        //        throw new Exception("Property cannot be read. There is no more data to read.");

        //    if (jsonTextReader.TokenType != JsonToken.String)
        //        throw new Exception("The read token is not a guid.");

        //    if (!(jsonTextReader.Value is string value))
        //        throw new Exception("The read token is not a guid.");

        //    try
        //    {
        //        return new Guid(value);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("The read token is not a guid.", ex);
        //    }
        //}
    }
}