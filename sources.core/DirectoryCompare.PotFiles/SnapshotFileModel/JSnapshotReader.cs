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
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.JFiles.SnapshotFileModel
{
    public sealed class JSnapshotReader : JReader
    {
        public JSnapshotFieldType CurrentPropertyType { get; private set; } = JSnapshotFieldType.None;

        public JSnapshotReader(JsonTextReader jsonTextReader)
            : base(jsonTextReader)
        {
        }

        public bool MoveNext()
        {
            MoveToNextState();

            try
            {
                bool success = MoveToNextProperty();

                if (success)
                {
                    CurrentPropertyType = CalculateFieldType();
                    return true;
                }
                else
                {
                    CurrentPropertyType = JSnapshotFieldType.None;
                    return false;
                }
            }
            catch
            {
                CurrentPropertyType = JSnapshotFieldType.None;
                throw;
            }
        }

        private JSnapshotFieldType CalculateFieldType()
        {
            return JsonTextReader.Value switch
            {
                "serializer-id" => JSnapshotFieldType.SerializerId,
                "original-path" => JSnapshotFieldType.OriginalPath,
                "creation-time" => JSnapshotFieldType.CreationTime,
                _ => throw new Exception("Invalid field in directory object.")
            };
        }

        public Guid ReadSerializerId()
        {
            if (CurrentPropertyType != JSnapshotFieldType.SerializerId)
                throw new Exception("Current property is not the serializer id.");

            string rawValue = JsonTextReader.ReadAsString();

            return Guid.Parse(rawValue);
        }

        public string ReadOriginalPath()
        {
            if (CurrentPropertyType != JSnapshotFieldType.OriginalPath)
                throw new Exception("Current property is not the original path.");

            return JsonTextReader.ReadAsString();
        }

        public DateTime ReadCreationTime()
        {
            if (CurrentPropertyType != JSnapshotFieldType.CreationTime)
                throw new Exception("Current property is not the creation time.");

            DateTime? readAsDateTime = JsonTextReader.ReadAsDateTime();
            return readAsDateTime.Value;
        }

        public IEnumerable<JFileReader> ReadFiles()
        {
            if (CurrentPropertyType != JSnapshotFieldType.FileCollection)
                throw new Exception("Current property is not the file collection.");

            return ReadFilesCollection();
        }

        public IEnumerable<JDirectoryReader> ReadSubDirectories()
        {
            if (CurrentPropertyType != JSnapshotFieldType.DirectoryCollection)
                throw new Exception("Current property is not the sub-directory collection.");

            return ReadDirectoriesCollection();
        }
    }
}