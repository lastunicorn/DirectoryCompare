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
using DustInTheWind.DirectoryCompare.Entities;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.JsonHashesFile.JsonExport
{
    public class JsonDiskAnalysisExport : IDiskAnalysisExport
    {
        private readonly Stack<JsonDirectory> directoryStack = new Stack<JsonDirectory>();
        private JsonSnapshot jsonSnapshot;

        private readonly JsonTextWriter jsonTextWriter;

        public Guid Id => new Guid("9E93055D-7BDE-4F55-B340-DD5A4880D96E");

        public JsonDiskAnalysisExport(TextWriter textWriter)
        {
            jsonTextWriter = new JsonTextWriter(textWriter)
            {
                Formatting = Formatting.Indented
            };
        }

        public void Open(string originalPath)
        {
            jsonSnapshot = new JsonSnapshot(jsonTextWriter)
            {
                Id = Id,
                OriginalPath = originalPath
            };

            jsonSnapshot.WriteStart();
        }

        public void Open(Snapshot snapshot)
        {
            jsonSnapshot = new JsonSnapshot(jsonTextWriter)
            {
                Id = Id,
                OriginalPath = snapshot.OriginalPath,
                CreationTime = snapshot.CreationTime
            };

            jsonSnapshot.WriteStart();
        }

        public void OpenNewDirectory(HDirectory directory)
        {
            if (directoryStack.Count == 0)
            {
                directoryStack.Push(jsonSnapshot);
            }
            else
            {
                JsonDirectory topDirectory = directoryStack.Peek();
                JsonDirectory newDirectory = topDirectory.WriteStartDirectory(directory);
                directoryStack.Push(newDirectory);
            }
        }

        public void CloseDirectory()
        {
            JsonDirectory topDirectory = directoryStack.Pop();
            topDirectory.WriteEnd();
        }

        public void Add(HFile file)
        {
            JsonDirectory topDirectory = directoryStack.Peek();
            topDirectory.WriteFile(file);
        }

        public void Add(HDirectory directory)
        {
            JsonDirectory topDirectory = directoryStack.Peek();
            JsonDirectory newDirectory = topDirectory.WriteStartDirectory(directory);
            newDirectory.WriteEnd();
        }

        public void Close()
        {
            //jsonSnapshot.WriteEnd();
            jsonTextWriter.Flush();
        }
    }
}