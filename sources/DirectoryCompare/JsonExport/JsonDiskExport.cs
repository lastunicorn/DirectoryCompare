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
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.JsonExport
{
    public class JsonDiskExport : IDiskExport
    {
        private readonly JsonTextWriter jsonTextWriter;

        public Guid Id => new Guid("9E93055D-7BDE-4F55-B340-DD5A4880D96E");

        private readonly Stack<JsonDirectoryExport> stack = new Stack<JsonDirectoryExport>();
        private JsonContainerExport jsonContainerExport;

        public JsonDiskExport(TextWriter textWriter)
        {
            jsonTextWriter = new JsonTextWriter(textWriter)
            {
                Formatting = Formatting.Indented
            };
        }

        public void Open(string originalPath)
        {
            jsonContainerExport = new JsonContainerExport(jsonTextWriter)
            {
                Id = Id,
                OriginalPath = originalPath
            };
        }

        public void OpenNewDirectory(XDirectory xDirectory)
        {
            if (stack.Count == 0)
            {
                jsonContainerExport.Open(xDirectory);
                stack.Push(jsonContainerExport);
            }
            else
            {
                JsonDirectoryExport jsonDirectoryExport = stack.Peek().OpenNewDirectory(xDirectory);
                stack.Push(jsonDirectoryExport);
            }
        }

        public void CloseDirectory()
        {
            JsonDirectoryExport jsonDirectoryExport = stack.Pop();
            jsonDirectoryExport.CloseDirectory();
        }

        public void Add(XFile xFile)
        {
            stack.Peek().Add(xFile);
        }

        public void Add(XDirectory xDirectory)
        {
            JsonDirectoryExport jsonDirectoryExport = stack.Peek().OpenNewDirectory(xDirectory);
            jsonDirectoryExport.CloseDirectory();
        }

        public void Close()
        {
            jsonTextWriter.Close();
        }
    }
}