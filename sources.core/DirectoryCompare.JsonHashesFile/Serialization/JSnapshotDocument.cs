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

using System.IO;
using DustInTheWind.DirectoryCompare.Domain.DiskAnalysis;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.JsonHashesFile.JsonExport;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.JsonHashesFile.Serialization
{
    public class JSnapshotDocument
    {
        public Snapshot Snapshot { get; set; }

        public void Save(string destinationFilePath)
        {
            using StreamWriter streamWriter = new StreamWriter(destinationFilePath);
            Save(streamWriter);
        }

        public void Save(StreamWriter streamWriter)
        {
            JsonAnalysisExport jsonAnalysisExport = new JsonAnalysisExport(streamWriter);

            jsonAnalysisExport.Open(Snapshot);

            SaveDirectory(jsonAnalysisExport, Snapshot);

            jsonAnalysisExport.Close();
        }

        private void SaveDirectory(IAnalysisExport analysisExport, HDirectory directory)
        {
            analysisExport.AddAndOpen(directory);

            foreach (HDirectory subDirectory in directory.Directories)
                SaveDirectory(analysisExport, subDirectory);

            foreach (HFile file in directory.Files)
                analysisExport.Add(file);

            analysisExport.CloseDirectory();
        }

        public static JSnapshotDocument Load(string sourceFilePath)
        {
            using (StreamReader streamReader = File.OpenText(sourceFilePath))
            using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
            {
                JsonSerializer serializer = new JsonSerializer();
                JSnapshot jSnapshot = (JSnapshot)serializer.Deserialize(jsonTextReader, typeof(JSnapshot));

                return new JSnapshotDocument
                {
                    Snapshot = jSnapshot.ToSnapshot()
                };
            }
        }
    }
}