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
using System.Text.RegularExpressions;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.JsonHashesFile.Serialization;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.JsonHashesFile
{
    public class SnapshotFile
    {
        private readonly string filePath;

        private JsonSnapshot jsonSnapshot;

        public bool Exists => filePath != null && File.Exists(filePath);

        public DateTime? CreationTime { get; }

        public Snapshot Snapshot
        {
            get => jsonSnapshot.ToSnapshot();
            set => jsonSnapshot = new JsonSnapshot(value);
        }

        public SnapshotFile(string filePath)
        {
            this.filePath = filePath;

            if (filePath != null)
            {
                string dateString = Path.GetFileNameWithoutExtension(filePath);


                Regex regex = new Regex(@"^([1-9]\d*)\s(0[1-9]|1[0-2])\s(0[1-9]|[12]\d|3[01])\s([0-5]\d)([0-5]\d)([0-5]\d)$", RegexOptions.Singleline);
                Match match = regex.Match(dateString);

                if (match.Success)
                {
                    int year = int.Parse(match.Groups[1].Value);
                    int month = int.Parse(match.Groups[2].Value);
                    int day = int.Parse(match.Groups[3].Value);
                    int hour = int.Parse(match.Groups[4].Value);
                    int minute = int.Parse(match.Groups[5].Value);
                    int second = int.Parse(match.Groups[6].Value);

                    CreationTime = new DateTime(year, month, day, hour, minute, second);
                }
            }
        }

        public void Open()
        {
            if (!File.Exists(filePath))
                return;

            using (StreamReader streamReader = File.OpenText(filePath))
            using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
            {
                JsonSerializer serializer = new JsonSerializer();
                jsonSnapshot = (JsonSnapshot)serializer.Deserialize(jsonTextReader, typeof(JsonSnapshot));
            }
        }

        public void Save()
        {
            using (FileStream stream = File.OpenWrite(filePath))
            using (StreamWriter streamWriter = new StreamWriter(stream))
            using (JsonTextWriter jsonTextWriter = new JsonTextWriter(streamWriter))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(jsonTextWriter, jsonSnapshot);
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