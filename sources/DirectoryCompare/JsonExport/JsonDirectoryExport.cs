// DirectoryCompare
// Copyright (C) 2017 Dust in the Wind
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

namespace DustInTheWind.DirectoryCompare.JsonExport
{
    internal class JsonDirectoryExport
    {
        protected JsonTextWriter Writer { get; }

        private JsonNodeState directoriesNodeState;
        private JsonNodeState filesPropertyNodeState;

        public JsonDirectoryExport(JsonTextWriter jsonTextWriter)
        {
            Writer = jsonTextWriter ?? throw new ArgumentNullException(nameof(jsonTextWriter));
        }

        public void Open(XDirectory xDirectory)
        {
            DoOpen(xDirectory);

            directoriesNodeState = JsonNodeState.NotOpened;
            filesPropertyNodeState = JsonNodeState.NotOpened;
        }

        protected virtual void DoOpen(XDirectory xDirectory)
        {
            Writer.WriteStartObject();

            Writer.WritePropertyName("n");
            Writer.WriteValue(xDirectory.Name);
        }

        public void Add(XFile xFile)
        {
            CloseDirectoriesNode();
            OpenFilesNode();

            Writer.WriteStartObject();
            Writer.WritePropertyName("n");
            Writer.WriteValue(xFile.Name);
            Writer.WritePropertyName("h");
            Writer.WriteValue(xFile.Hash);
            Writer.WriteEndObject();
        }

        private void OpenFilesNode()
        {
            if (filesPropertyNodeState == JsonNodeState.NotOpened)
            {
                Writer.WritePropertyName("f");
                Writer.WriteStartArray();

                filesPropertyNodeState = JsonNodeState.Opened;
            }
            else if (filesPropertyNodeState == JsonNodeState.Closed)
            {
                throw new Exception("Files list is already closed.");
            }
        }

        private void CloseFilesNode()
        {
            if (filesPropertyNodeState == JsonNodeState.Opened)
            {
                Writer.WriteEndArray();
                filesPropertyNodeState = JsonNodeState.Closed;
            }
        }

        public JsonDirectoryExport OpenNewDirectory(XDirectory xDirectory)
        {
            CloseFilesNode();
            OpenDirectoriesNode();

            JsonDirectoryExport jsonDirectoryExport = new JsonDirectoryExport(Writer);
            jsonDirectoryExport.Open(xDirectory);

            return jsonDirectoryExport;
        }

        private void OpenDirectoriesNode()
        {
            if (directoriesNodeState == JsonNodeState.NotOpened)
            {
                Writer.WritePropertyName("d");
                Writer.WriteStartArray();

                directoriesNodeState = JsonNodeState.Opened;
            }
            else if (directoriesNodeState == JsonNodeState.Closed)
            {
                throw new Exception("Directories list is already closed.");
            }
        }

        private void CloseDirectoriesNode()
        {
            if (directoriesNodeState == JsonNodeState.Opened)
            {
                Writer.WriteEndArray();
                directoriesNodeState = JsonNodeState.Closed;
            }
        }

        public void CloseDirectory()
        {
            CloseFilesNode();
            CloseDirectoriesNode();

            Writer.WriteEndObject();
        }
    }
}