using System;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.JsonHashesFile.JsonExport
{
    internal class JsonFileWriter
    {
        protected JsonTextWriter Writer { get; }

        public JsonFileWriter(JsonTextWriter jsonTextWriter)
        {
            Writer = jsonTextWriter ?? throw new ArgumentNullException(nameof(jsonTextWriter));
        }

        public void Write(HFile file)
        {
            Writer.WriteStartObject();

            Writer.WritePropertyName("n");
            Writer.WriteValue(file.Name);

            Writer.WritePropertyName("s");
            Writer.WriteValue(file.Size);

            Writer.WritePropertyName("m");
            Writer.WriteValue(file.LastModifiedTime);

            Writer.WritePropertyName("h");
            Writer.WriteValue(file.Hash);

            Writer.WriteEndObject();
        }
    }
}