using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DustInTheWind.DirectoryCompare.Serialization
{
    internal class SerializersPool
    {
        private const string SerializersDirectoryName = "Serializers";

        private List<ISerializer> serializers = new List<ISerializer>();

        private SerializersPool()
        {
            string entryAssemblyFilePath = Assembly.GetEntryAssembly().Location;
            string applicationDirectoryPath = Path.GetDirectoryName(entryAssemblyFilePath);

            string serializersDirectoryPath = Path.Combine(applicationDirectoryPath, SerializersDirectoryName);

            string[] files = Directory.GetFiles(serializersDirectoryPath, "*.dll");



            foreach (string file in files)
            {
                Assembly assembly = Assembly.LoadFrom(file);

                IEnumerable<ISerializer> newSerializers = assembly.GetTypes()
                    .Where(x => typeof(ISerializer).IsAssignableFrom(x))
                    .Select(Activator.CreateInstance)
                    .Cast<ISerializer>();

                serializers.AddRange(newSerializers);
            }
        }
    }
}