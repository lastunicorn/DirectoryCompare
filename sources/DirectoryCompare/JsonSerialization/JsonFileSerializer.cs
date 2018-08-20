using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.JsonSerialization
{
    public class JsonFileSerializer
    {
        public void WriteToFile(Container container, string destinationFilePath)
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

        public Container ReadFromFile(string sourceFilePath)
        {
            string json = File.ReadAllText(sourceFilePath);
            JsonXContainer jsonXContainer = JsonConvert.DeserializeObject<JsonXContainer>(json);

            return jsonXContainer.ToContainer();
        }
    }
}