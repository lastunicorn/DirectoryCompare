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

using System;
using System.IO;
using DustInTheWind.DirectoryCompare.JFiles.SnapshotFileModel;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.JFiles
{
    public class SnapshotFile
    {
        private readonly SnapshotFilePath filePath;

        public bool Exists => filePath != null && File.Exists(filePath);

        public DateTime? CreationTime => filePath.CreationTime;

        public JSnapshot Snapshot { get; set; }

        public SnapshotFile(SnapshotFilePath filePath)
        {
            this.filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        public void Open()
        {
            if (!File.Exists(filePath))
                return;

            using (StreamReader streamReader = File.OpenText(filePath))
            using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
            {
                JsonSerializer serializer = new JsonSerializer();
                Snapshot = (JSnapshot)serializer.Deserialize(jsonTextReader, typeof(JSnapshot));
            }
        }

        public void Save()
        {
            using (FileStream stream = File.OpenWrite(filePath))
            using (StreamWriter streamWriter = new StreamWriter(stream))
            using (JsonTextWriter jsonTextWriter = new JsonTextWriter(streamWriter))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(jsonTextWriter, Snapshot);
            }
        }

        public void Delete()
        {
            File.Delete(filePath);
        }

        public Stream OpenStream()
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            Directory.CreateDirectory(directoryPath);

            return new FileStream(filePath, FileMode.CreateNew);
        }
    }
}