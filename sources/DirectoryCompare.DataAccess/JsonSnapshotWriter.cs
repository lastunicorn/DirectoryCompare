// DirectoryCompare
// Copyright (C) 2017-2023 Dust in the Wind
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

using DustInTheWind.DirectoryCompare.DataAccess.PotFiles.SnapshotFileModel;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Domain.ImportExport;

namespace DustInTheWind.DirectoryCompare.DataAccess;

public sealed class JsonSnapshotWriter : ISnapshotWriter
{
    private readonly JSnapshotWriter jSnapshotWriter;
    private readonly Stack<JDirectoryWriter> directoryStack = new();

    private static Guid Id => new("9E93055D-7BDE-4F55-B340-DD5A4880D96E");

    public JsonSnapshotWriter(JSnapshotWriter jSnapshotWriter)
    {
        this.jSnapshotWriter = jSnapshotWriter;
    }

    public void Write(Snapshot snapshot)
    {
        Open(snapshot.OriginalPath, snapshot.CreationTime);

        foreach (HDirectory subDirectory in snapshot.Directories)
            SaveDirectory(subDirectory);

        foreach (HFile file in snapshot.Files)
            Add(file);

        Close();
    }

    private void SaveDirectory(HDirectory directory)
    {
        AddAndOpen(directory);

        foreach (HDirectory subDirectory in directory.Directories)
            SaveDirectory(subDirectory);

        foreach (HFile file in directory.Files)
            Add(file);

        CloseDirectory();
    }

    public void Open(string originalPath, Guid analysisId)
    {
        jSnapshotWriter.WriteStart();
        jSnapshotWriter.WriteSerializerId(Id);
        jSnapshotWriter.WriteAnalysisId(analysisId);
        jSnapshotWriter.WriteOriginalPath(originalPath);
        jSnapshotWriter.WriteCreationTime(DateTime.UtcNow);

        directoryStack.Push(jSnapshotWriter);
    }

    public void Open(string originalPath, DateTime creationTime)
    {
        jSnapshotWriter.WriteStart();
        jSnapshotWriter.WriteSerializerId(Id);
        jSnapshotWriter.WriteOriginalPath(originalPath);
        jSnapshotWriter.WriteCreationTime(creationTime);

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
        while (directoryStack.Count > 0)
        {
            JDirectoryWriter jDirectoryWriter = directoryStack.Pop();
            jDirectoryWriter.WriteEnd();
        }
    }

    public void Dispose()
    {
        jSnapshotWriter?.Dispose();
    }
}