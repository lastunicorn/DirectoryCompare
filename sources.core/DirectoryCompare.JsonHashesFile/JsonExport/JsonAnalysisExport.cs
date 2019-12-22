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
using DustInTheWind.DirectoryCompare.DiskAnalysis;
using DustInTheWind.DirectoryCompare.Entities;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.JsonHashesFile.JsonExport
{
    public class JsonAnalysisExport : IAnalysisExport
    {
        private readonly Stack<JsonDirectoryWriter> directoryStack = new Stack<JsonDirectoryWriter>();
        private JsonSnapshotWriter jsonSnapshotWriter;

        private readonly JsonTextWriter jsonTextWriter;

        public Guid Id => new Guid("9E93055D-7BDE-4F55-B340-DD5A4880D96E");

        public JsonAnalysisExport(TextWriter textWriter)
        {
            jsonTextWriter = new JsonTextWriter(textWriter)
            {
                Formatting = Formatting.Indented
            };
        }

        public void Open(string originalPath)
        {
            jsonSnapshotWriter = new JsonSnapshotWriter(jsonTextWriter)
            {
                Id = Id,
                OriginalPath = originalPath,
                CreationTime = DateTime.UtcNow
            };

            jsonSnapshotWriter.WriteStart();
        }

        public void Open(Snapshot snapshot)
        {
            jsonSnapshotWriter = new JsonSnapshotWriter(jsonTextWriter)
            {
                Id = Id,
                OriginalPath = snapshot.OriginalPath,
                CreationTime = snapshot.CreationTime
            };

            jsonSnapshotWriter.WriteStart();
        }

        public void AddAndOpen(HDirectory directory)
        {
            if (directoryStack.Count == 0)
            {
                directoryStack.Push(jsonSnapshotWriter);
            }
            else
            {
                JsonDirectoryWriter topDirectoryWriter = directoryStack.Peek();
                JsonDirectoryWriter newDirectoryWriter = topDirectoryWriter.WriteStartDirectory(directory.Name);
                directoryStack.Push(newDirectoryWriter);
            }
        }

        public void CloseDirectory()
        {
            JsonDirectoryWriter topDirectoryWriter = directoryStack.Pop();
            topDirectoryWriter.WriteEnd();
        }

        public void Add(HFile file)
        {
            JsonDirectoryWriter topDirectoryWriter = directoryStack.Peek();
            topDirectoryWriter.WriteFile(file);
        }

        public void Add(HDirectory directory)
        {
            JsonDirectoryWriter topDirectoryWriter = directoryStack.Peek();
            JsonDirectoryWriter newDirectoryWriter = topDirectoryWriter.WriteStartDirectory(directory.Name);
            newDirectoryWriter.WriteEnd();
        }

        public void Close()
        {
            //jsonSnapshotWriter.WriteEnd();
            jsonTextWriter.Flush();
        }
    }
}