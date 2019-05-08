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

using System;
using DustInTheWind.DirectoryCompare.Entities;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.JsonHashesFile.JsonExport
{
    internal class JsonDirectory
    {
        protected JsonTextWriter Writer { get; }

        private JsonNodeState directoriesNodeState;
        private JsonNodeState filesPropertyNodeState;

        public JsonDirectory(JsonTextWriter jsonTextWriter)
        {
            Writer = jsonTextWriter ?? throw new ArgumentNullException(nameof(jsonTextWriter));
        }

        public void WriteStart(HDirectory directory)
        {
            WriteStartDirectoryInternal(directory);

            directoriesNodeState = JsonNodeState.NotOpened;
            filesPropertyNodeState = JsonNodeState.NotOpened;
        }

        protected virtual void WriteStartDirectoryInternal(HDirectory directory)
        {
            Writer.WriteStartObject();

            Writer.WritePropertyName("n");
            Writer.WriteValue(directory.Name);
        }

        public void WriteFile(HFile xFile)
        {
            WriteEndDirectoriesArray();
            WriteStartFilesArray();

            Writer.WriteStartObject();

            Writer.WritePropertyName("n");
            Writer.WriteValue(xFile.Name);

            Writer.WritePropertyName("h");
            Writer.WriteValue(xFile.Hash);

            Writer.WriteEndObject();
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

        private void WriteEndFilesArray()
        {
            if (filesPropertyNodeState != JsonNodeState.Opened)
                return;

            Writer.WriteEndArray();
            filesPropertyNodeState = JsonNodeState.Closed;
        }

        public JsonDirectory OpenNewDirectory(HDirectory directory)
        {
            WriteEndFilesArray();
            WriteStartDirectoriesArray();

            JsonDirectory jsonDirectory = new JsonDirectory(Writer);
            jsonDirectory.WriteStart(directory);

            return jsonDirectory;
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

        public void CloseDirectory()
        {
            WriteEndFilesArray();
            WriteEndDirectoriesArray();

            Writer.WriteEndObject();
        }
    }
}