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
using DustInTheWind.DirectoryCompare.DataAccess.Transformations;
using DustInTheWind.DirectoryCompare.Domain.DiskAnalysis;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Domain.ImportExport;
using DustInTheWind.DirectoryCompare.JFiles.SnapshotFileModel;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.DataAccess
{
    public class PotImportExport : IPotImportExport
    {
        public Snapshot Import(string filePath)
        {
            using (StreamReader streamReader = File.OpenText(filePath))
            using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
            {
                JsonSerializer serializer = new JsonSerializer();
                JSnapshot jSnapshot = (JSnapshot)serializer.Deserialize(jsonTextReader, typeof(JSnapshot));

                return jSnapshot.ToSnapshot();
            }
        }

        public void Export(Snapshot snapshot, string filePath)
        {
            using StreamWriter streamWriter = new StreamWriter(filePath);
            Export(snapshot, streamWriter);

        }
        public void Export(Snapshot snapshot, StreamWriter streamWriter)
        {
            JsonAnalysisExport jsonAnalysisExport = new JsonAnalysisExport(streamWriter);

            jsonAnalysisExport.Open(snapshot);

            foreach (HDirectory subDirectory in snapshot.Directories)
                SaveDirectory(jsonAnalysisExport, subDirectory);

            foreach (HFile file in snapshot.Files)
                jsonAnalysisExport.Add(file);

            jsonAnalysisExport.Close();
        }

        private static void SaveDirectory(IAnalysisExport analysisExport, HDirectory directory)
        {
            analysisExport.AddAndOpen(directory);

            foreach (HDirectory subDirectory in directory.Directories)
                SaveDirectory(analysisExport, subDirectory);

            foreach (HFile file in directory.Files)
                analysisExport.Add(file);

            analysisExport.CloseDirectory();
        }
    }
}