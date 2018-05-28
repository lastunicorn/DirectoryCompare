using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.JsonSerialization
{
    internal class JsonXContainer
    {
        [JsonProperty("original-path")]
        public string OriginalPath { get; set; }

        [JsonProperty("creation-time")]
        public DateTime CreationTime { get; set; }

        [JsonProperty("d")]
        public List<JsonXDirectory> Directories { get; set; }

        [JsonProperty("f")]
        public List<JsonXFile> Files { get; set; }

        public JsonXContainer()
        {
        }

        public JsonXContainer(Container container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            OriginalPath = container.OriginalPath;
            CreationTime = container.CreationTime;

            if (container.Directories == null)
                Directories = null;
            else
                Directories = container.Directories
                    .Select(x => new JsonXDirectory(x))
                    .ToList();

            if (container.Files == null)
                Files = null;
            else
                Files = container.Files
                    .Select(x => new JsonXFile(x))
                    .ToList();
        }
    }
}