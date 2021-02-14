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

namespace DustInTheWind.DirectoryCompare.JsonHashesFile.JsonExport
{
    public class JDirectoryWriter
    {
        protected JsonTextWriter Writer { get; }

        private JsonNodeState directoriesNodeState;
        private JsonNodeState filesPropertyNodeState;

        public JDirectoryWriter(JsonTextWriter jsonTextWriter)
        {
            Writer = jsonTextWriter ?? throw new ArgumentNullException(nameof(jsonTextWriter));

            directoriesNodeState = JsonNodeState.NotOpened;
            filesPropertyNodeState = JsonNodeState.NotOpened;
        }

        public void WriteStart()
        {
            Writer.WriteStartObject();
        }

        public void WriteName(string directoryName)
        {
            Writer.WritePropertyName("n");
            Writer.WriteValue(directoryName);
        }

        public JFileWriter CreateFile()
        {
            WriteEndDirectoriesArray();
            WriteStartFilesArray();

            return new JFileWriter(Writer);
        }

        private void WriteStartFilesArray()
        {
            switch (filesPropertyNodeState)
            {
                case JsonNodeState.NotOpened:
                    Writer.WritePropertyName("f");
                    Writer.WriteStartArray();

                    filesPropertyNodeState = JsonNodeState.Opened;
                    break;

                case JsonNodeState.Closed:
                    throw new Exception("Files list is already closed. No more files can be added.");

                case JsonNodeState.Opened:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public JDirectoryWriter CreateSubDirectory()
        {
            WriteEndFilesArray();
            WriteStartDirectoriesArray();

            return new JDirectoryWriter(Writer);
        }

        private void WriteEndFilesArray()
        {
            if (filesPropertyNodeState != JsonNodeState.Opened)
                return;

            Writer.WriteEndArray();
            filesPropertyNodeState = JsonNodeState.Closed;
        }

        private void WriteStartDirectoriesArray()
        {
            switch (directoriesNodeState)
            {
                case JsonNodeState.NotOpened:
                    Writer.WritePropertyName("d");
                    Writer.WriteStartArray();

                    directoriesNodeState = JsonNodeState.Opened;
                    break;

                case JsonNodeState.Closed:
                    throw new Exception("Directories list is already closed. No more directories can be added.");

                case JsonNodeState.Opened:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void WriteEndDirectoriesArray()
        {
            if (directoriesNodeState != JsonNodeState.Opened)
                return;

            Writer.WriteEndArray();
            directoriesNodeState = JsonNodeState.Closed;
        }

        public void WriteEnd()
        {
            WriteEndFilesArray();
            WriteEndDirectoriesArray();

            Writer.WriteEndObject();
        }
    }
}