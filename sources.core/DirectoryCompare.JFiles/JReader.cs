using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.JsonHashesFile.JsonExport
{
    public class JReader
    {
        protected readonly JsonTextReader jsonTextReader;

        protected JReaderState state = JReaderState.New;

        protected JReader(JsonTextReader jsonTextReader)
        {
            this.jsonTextReader = jsonTextReader ?? throw new ArgumentNullException(nameof(jsonTextReader));
        }

        protected bool MoveToNextProperty()
        {
            while (true)
            {
                bool success = jsonTextReader.Read();

                if (!success)
                    return false;

                switch (jsonTextReader.TokenType)
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
                        state = JReaderState.Finished;
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
                bool success = jsonTextReader.Read();

                if (!success)
                    return false;

                switch (jsonTextReader.TokenType)
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
                bool success = jsonTextReader.Read();

                if (!success)
                    return false;

                switch (jsonTextReader.TokenType)
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
            bool isFProperty = jsonTextReader.TokenType == JsonToken.PropertyName && jsonTextReader.ReadAsString() == "f";

            if (isFProperty)
            {
                bool success = MoveToArrayStart();

                if (!success)
                    yield break;
            }

            bool isArrayStart = jsonTextReader.TokenType == JsonToken.StartArray;

            if (!isArrayStart)
                yield break;

            while (true)
            {
                bool success = MoveToObjectStart();

                if (!success)
                    yield break;

                yield return new JFileReader(jsonTextReader);
            }
        }

        protected IEnumerable<JDirectoryReader> ReadDirectoriesCollection()
        {
            bool isDProperty = jsonTextReader.TokenType == JsonToken.PropertyName && jsonTextReader.ReadAsString() == "d";

            if (isDProperty)
            {
                bool success = MoveToArrayStart();

                if (!success)
                    yield break;
            }

            bool isArrayStart = jsonTextReader.TokenType == JsonToken.StartArray;

            if (!isArrayStart)
                yield break;

            while (true)
            {
                bool success = MoveToObjectStart();

                if (!success)
                    yield break;

                yield return new JDirectoryReader(jsonTextReader);
            }
        }
    }
}