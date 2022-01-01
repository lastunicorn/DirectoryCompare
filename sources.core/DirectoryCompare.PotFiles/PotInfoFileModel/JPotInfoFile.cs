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
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.JFiles.PotInfoFileModel
{
    public class JPotInfoFile
    {
        private readonly string filePath;

        public JPotInfo JPotInfo { get; set; }

        public bool Exists => File.Exists(filePath);

        public bool IsValid => JPotInfo != null;

        public JPotInfoFile(string filePath)
        {
            this.filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        public bool TryOpen()
        {
            try
            {
                return Open();
            }
            catch
            {
                return false;
            }
        }

        public bool Open()
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                JPotInfo = JsonConvert.DeserializeObject<JPotInfo>(json);
                return true;
            }
            else
            {
                JPotInfo = null;
                return false;
            }
        }

        public void Save()
        {
            string json = JsonConvert.SerializeObject(JPotInfo, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
    }
}