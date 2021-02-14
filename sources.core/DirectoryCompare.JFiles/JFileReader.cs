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
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.JFiles
{
    public class JFileReader : JReader
    {
        private JReaderState state = JReaderState.New;

        public JFileFieldType CurrentPropertyType { get; private set; } = JFileFieldType.None;

        public JFileReader(JsonTextReader jsonTextReader)
            : base(jsonTextReader)
        {
        }

        public JFileFieldType MoveToNext()
        {
            switch (state)
            {
                case JReaderState.New:
                    state = JReaderState.InProgress;
                    break;

                case JReaderState.InProgress:
                    break;

                case JReaderState.Finished:
                    throw new Exception("The reader already finished reading the json object.");

                default:
                    throw new Exception("Invalid reader state.");
            }

            try
            {
                bool success = MoveToNextProperty();

                CurrentPropertyType = success
                    ? jsonTextReader.Value switch
                    {
                        "n" => JFileFieldType.FileName,
                        "s" => JFileFieldType.FileSize,
                        "m" => JFileFieldType.LastModifiedTime,
                        "h" => JFileFieldType.Hash,
                        _ => throw new Exception("Invalid field in file object.")
                    }
                    : JFileFieldType.None;

                return CurrentPropertyType;
            }
            catch
            {
                CurrentPropertyType = JFileFieldType.None;
                throw;
            }
        }

        public string ReadName()
        {
            if (CurrentPropertyType != JFileFieldType.FileName)
                throw new Exception("Current property is not the file name.");

            return jsonTextReader.ReadAsString();
        }

        public ulong ReadSize()
        {
            if (CurrentPropertyType != JFileFieldType.FileSize)
                throw new Exception("Current property is not the file size.");

            string rawValue = jsonTextReader.ReadAsString();
            return ulong.Parse(rawValue);
        }

        public DateTime ReadLastModifiedTime()
        {
            if (CurrentPropertyType != JFileFieldType.LastModifiedTime)
                throw new Exception("Current property is not the last modified time.");

            DateTime? readAsDateTime = jsonTextReader.ReadAsDateTime();
            return readAsDateTime.Value;
        }

        public byte[] ReadHash()
        {
            if (CurrentPropertyType != JFileFieldType.Hash)
                throw new Exception("Current property is not the file hash.");

            byte[] bytes = jsonTextReader.ReadAsBytes();
            return bytes;
        }
    }
}