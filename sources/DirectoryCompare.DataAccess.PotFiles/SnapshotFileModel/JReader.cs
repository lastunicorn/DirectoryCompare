// DirectoryCompare
// Copyright (C) 2017-2023 Dust in the Wind
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

using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.DataAccess.PotFiles.SnapshotFileModel;

public class JReader
{
    protected readonly JsonTextReader JsonTextReader;

    protected JReaderState State = JReaderState.New;

    protected JReader(JsonTextReader jsonTextReader)
    {
        JsonTextReader = jsonTextReader ?? throw new ArgumentNullException(nameof(jsonTextReader));
    }

    protected void MoveToNextState()
    {
        switch (State)
        {
            case JReaderState.New:
                State = JReaderState.InProgress;
                break;

            case JReaderState.InProgress:
                break;

            case JReaderState.Finished:
                throw new Exception("The reader already finished reading the json object.");

            default:
                throw new Exception("Invalid reader state.");
        }
    }

    protected bool MoveToNextProperty()
    {
        while (true)
        {
            bool success = JsonTextReader.Read();

            if (!success)
                return false;

            switch (JsonTextReader.TokenType)
            {
                case JsonToken.None:
                case JsonToken.StartObject:
                case JsonToken.StartArray:
                case JsonToken.StartConstructor:
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
                    throw new Exception("Invalid json token in directory object.");

                case JsonToken.Comment:
                    break;

                case JsonToken.PropertyName:
                    return true;

                case JsonToken.EndObject:
                    State = JReaderState.Finished;
                    return false;

                default:
                    throw new Exception("Invalid json token in directory object.");
            }
        }
    }

    protected bool MoveToArrayStart()
    {
        while (true)
        {
            bool success = JsonTextReader.Read();

            if (!success)
                return false;

            switch (JsonTextReader.TokenType)
            {
                case JsonToken.None:
                case JsonToken.StartObject:
                case JsonToken.StartConstructor:
                case JsonToken.PropertyName:
                case JsonToken.Raw:
                case JsonToken.Integer:
                case JsonToken.Float:
                case JsonToken.String:
                case JsonToken.Boolean:
                case JsonToken.Null:
                case JsonToken.Undefined:
                case JsonToken.EndObject:
                case JsonToken.EndArray:
                case JsonToken.EndConstructor:
                case JsonToken.Date:
                case JsonToken.Bytes:
                    throw new Exception("Invalid json token in files collection.");

                case JsonToken.StartArray:
                    return true;

                case JsonToken.Comment:
                    break;

                default:
                    throw new Exception("Invalid json token in files collection.");
            }
        }
    }

    protected bool MoveToObjectStart()
    {
        while (true)
        {
            bool success = JsonTextReader.Read();

            if (!success)
                return false;

            switch (JsonTextReader.TokenType)
            {
                case JsonToken.None:
                case JsonToken.StartArray:
                case JsonToken.StartConstructor:
                case JsonToken.PropertyName:
                case JsonToken.Raw:
                case JsonToken.Integer:
                case JsonToken.Float:
                case JsonToken.String:
                case JsonToken.Boolean:
                case JsonToken.Null:
                case JsonToken.Undefined:
                case JsonToken.EndObject:
                case JsonToken.EndArray:
                case JsonToken.EndConstructor:
                case JsonToken.Date:
                case JsonToken.Bytes:
                    throw new Exception("Invalid json token in files collection.");

                case JsonToken.StartObject:
                    return true;

                case JsonToken.Comment:
                    break;

                default:
                    throw new Exception("Invalid json token in files collection.");
            }
        }
    }

    protected IEnumerable<JFileReader> ReadFilesCollection()
    {
        bool isFProperty = JsonTextReader.TokenType == JsonToken.PropertyName && JsonTextReader.ReadAsString() == "f";

        if (isFProperty)
        {
            bool success = MoveToArrayStart();

            if (!success)
                yield break;
        }

        bool isArrayStart = JsonTextReader.TokenType == JsonToken.StartArray;

        if (!isArrayStart)
            yield break;

        while (true)
        {
            bool success = MoveToObjectStart();

            if (!success)
                yield break;

            yield return new JFileReader(JsonTextReader);
        }
    }

    protected IEnumerable<JDirectoryReader> ReadDirectoriesCollection()
    {
        bool isDProperty = JsonTextReader.TokenType == JsonToken.PropertyName && JsonTextReader.ReadAsString() == "d";

        if (isDProperty)
        {
            bool success = MoveToArrayStart();

            if (!success)
                yield break;
        }

        bool isArrayStart = JsonTextReader.TokenType == JsonToken.StartArray;

        if (!isArrayStart)
            yield break;

        while (true)
        {
            bool success = MoveToObjectStart();

            if (!success)
                yield break;

            yield return new JDirectoryReader(JsonTextReader);
        }
    }
}