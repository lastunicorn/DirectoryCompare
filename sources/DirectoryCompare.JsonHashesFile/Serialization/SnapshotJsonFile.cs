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

using System.IO;
using DustInTheWind.DirectoryCompare.Entities;
using DustInTheWind.DirectoryCompare.JsonHashesFile.JsonExport;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.JsonHashesFile.Serialization
{
    public class SnapshotJsonFile
    {
        public Snapshot Snapshot { get; set; }

        public void Save(string destinationFilePath)
        {
            using (StreamWriter streamWriter = new StreamWriter(destinationFilePath))
                Save(streamWriter);
        }

        public void Save(StreamWriter streamWriter)
        {
            JsonDiskAnalysisExport jsonDiskAnalysisExport = new JsonDiskAnalysisExport(streamWriter);

            jsonDiskAnalysisExport.Open(Snapshot);

            SaveDirectory(jsonDiskAnalysisExport, Snapshot);

            jsonDiskAnalysisExport.Close();
        }

        private void SaveDirectory(JsonDiskAnalysisExport jsonDiskAnalysisExport, HDirectory directory)
        {
            jsonDiskAnalysisExport.OpenNewDirectory(directory);

            foreach (HDirectory subDirectory in directory.Directories)
                SaveDirectory(jsonDiskAnalysisExport, subDirectory);

            foreach (HFile file in directory.Files)
                jsonDiskAnalysisExport.Add(file);

            jsonDiskAnalysisExport.CloseDirectory();
        }

        public static SnapshotJsonFile Load(string sourceFilePath)
        {
            using (StreamReader streamReader = File.OpenText(sourceFilePath))
            using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
            {
                JsonSerializer serializer = new JsonSerializer();
                JsonSnapshot jsonSnapshot = (JsonSnapshot)serializer.Deserialize(jsonTextReader, typeof(JsonSnapshot));

                return new SnapshotJsonFile
                {
                    Snapshot = jsonSnapshot.ToSnapshot()
                };
            }
        }

        ///// <summary>
        ///// Searches for the first property. It must be "ver" and contain the text "1.0".
        ///// </summary>
        //public bool CanDeserialize(string sourceFilePath)
        //{
        //    using (StreamReader streamReader = File.OpenText(sourceFilePath))
        //    using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
        //    {
        //        if (!jsonTextReader.Read())
        //            return false;

        //        if (jsonTextReader.TokenType != JsonToken.StartObject)
        //            return false;

        //        if (!jsonTextReader.Read())
        //            return false;

        //        if (jsonTextReader.TokenType != JsonToken.PropertyName)
        //            return false;

        //        if (jsonTextReader.Value as string != "serializer")
        //            return false;

        //        if (jsonTextReader.TokenType != JsonToken.StartObject)
        //            return false;

        //        if (!jsonTextReader.Read())
        //            return false;

        //        if (jsonTextReader.TokenType != JsonToken.PropertyName)
        //            return false;

        //        if (jsonTextReader.Value as string != "id")
        //            return false;

        //        if (!jsonTextReader.Read())
        //            return false;

        //        if (jsonTextReader.TokenType != JsonToken.String)
        //            return false;

        //        if (jsonTextReader.ValueType != typeof(string))
        //            return false;

        //        if (!(jsonTextReader.Value is string value))
        //            return false;

        //        try
        //        {
        //            return new Guid(value) == Id;
        //        }
        //        catch
        //        {
        //            return false;
        //        }
        //    }
        //}
    }
}