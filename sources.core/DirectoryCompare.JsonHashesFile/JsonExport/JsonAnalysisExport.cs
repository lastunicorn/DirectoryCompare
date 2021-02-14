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
using System.IO;
using DustInTheWind.DirectoryCompare.Domain.DiskAnalysis;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.JFiles;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.JsonHashesFile.JsonExport
{
    public class JsonAnalysisExport : IAnalysisExport
    {
        private readonly Stack<JDirectoryWriter> directoryStack = new Stack<JDirectoryWriter>();
        private JSnapshotWriter jSnapshotWriter;

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
            jSnapshotWriter = new JSnapshotWriter(jsonTextWriter);

            jSnapshotWriter.WriteStart();
            jSnapshotWriter.WriteId(Id);
            jSnapshotWriter.WriteOriginalPath(originalPath);
            jSnapshotWriter.WriteCreationTime(DateTime.UtcNow);

            directoryStack.Push(jSnapshotWriter);
        }

        public void Open(Snapshot snapshot)
        {
            jSnapshotWriter = new JSnapshotWriter(jsonTextWriter);

            jSnapshotWriter.WriteStart();
            jSnapshotWriter.WriteId(Id);
            jSnapshotWriter.WriteOriginalPath(snapshot.OriginalPath);
            jSnapshotWriter.WriteCreationTime(snapshot.CreationTime);

            directoryStack.Push(jSnapshotWriter);
        }

        public void Add(HDirectory directory)
        {
            JDirectoryWriter topDirectoryWriter = directoryStack.Peek();
            JDirectoryWriter newDirectoryWriter = topDirectoryWriter.CreateSubDirectory();
            newDirectoryWriter.WriteStart();
            newDirectoryWriter.WriteName(directory.Name);
            newDirectoryWriter.WriteEnd();
        }

        public void AddAndOpen(HDirectory directory)
        {
            JDirectoryWriter topDirectoryWriter = directoryStack.Peek();
            JDirectoryWriter newDirectoryWriter = topDirectoryWriter.CreateSubDirectory();
            newDirectoryWriter.WriteStart();
            newDirectoryWriter.WriteName(directory.Name);
            directoryStack.Push(newDirectoryWriter);
        }

        public void CloseDirectory()
        {
            JDirectoryWriter topDirectoryWriter = directoryStack.Pop();
            topDirectoryWriter.WriteEnd();
        }

        public void Add(HFile file)
        {
            JDirectoryWriter topDirectoryWriter = directoryStack.Peek();
            JFileWriter jFileWriter = topDirectoryWriter.CreateFile();
            jFileWriter.WriteStart();
            jFileWriter.WriteName(file.Name);
            jFileWriter.WriteSize(file.Size);
            jFileWriter.WriteLastModifiedTime(file.LastModifiedTime);
            jFileWriter.WriteHash(file.Hash);
            jFileWriter.WriteEnd();
        }

        public void Close()
        {
            //jsonSnapshotWriter.WriteEnd();
            jsonTextWriter.Flush();
        }
    }
}