using System;
using System.IO;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Domain.Utils;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.JsonHashesFile
{
    public class ProjectFile
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonIgnore]
        public DiskPath Path { get; set; }

        public static ProjectFile Create(Project project)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            return new ProjectFile
            {
                Name = project.Name,
                Path = project.Path + "project.json"
            };
        }

        public void Save()
        {
            string json = JsonConvert.SerializeObject(this);

            File.WriteAllText(Path, json);
        }
    }
}