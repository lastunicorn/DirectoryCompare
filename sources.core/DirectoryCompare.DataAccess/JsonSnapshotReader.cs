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
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.DataAccess;

public class JsonSnapshotReader : ISnapshotReader
{
    private readonly Stack<JDirectoryWriter> directoryStack = new();
    private JSnapshotReader jSnapshotReader;

    public Guid Id => new("9E93055D-7BDE-4F55-B340-DD5A4880D96E");

    public JsonSnapshotReader(TextReader textReader)
    {
        JsonTextReader jsonTextReader = new(textReader);
        jSnapshotReader = new JSnapshotReader(jsonTextReader);
    }

    public SnapshotItemType CurrentItemType
    {
        get
        {
            switch (jSnapshotReader.CurrentPropertyType)
            {
                case JSnapshotFieldType.None:
                    return SnapshotItemType.None;

                case JSnapshotFieldType.SerializerId:
                case JSnapshotFieldType.OriginalPath:
                case JSnapshotFieldType.CreationTime:
                    return SnapshotItemType.Info;

                case JSnapshotFieldType.FileCollection:
                    return SnapshotItemType.FileCollection;

                case JSnapshotFieldType.DirectoryCollection:
                    return SnapshotItemType.DirectoryCollection;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public bool MoveNext()
    {
        return jSnapshotReader.MoveNext();
    }

    public SnapshotHeader ReadHeader()
    {
        Guid? serializerId = null;
        string originalPath = null;
        DateTime? creationTime = null;

        bool isFinished = false;

        while (!isFinished)
        {
            switch (jSnapshotReader.CurrentPropertyType)
            {
                case JSnapshotFieldType.None:
                case JSnapshotFieldType.FileCollection:
                case JSnapshotFieldType.DirectoryCollection:
                    isFinished = true;
                    break;

                case JSnapshotFieldType.SerializerId:
                    serializerId = jSnapshotReader.ReadSerializerId();
                    break;

                case JSnapshotFieldType.OriginalPath:
                    originalPath = jSnapshotReader.ReadOriginalPath();
                    break;

                case JSnapshotFieldType.CreationTime:
                    creationTime = jSnapshotReader.ReadCreationTime();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            jSnapshotReader.MoveNext();
        }

        if (serializerId == null || originalPath == null || creationTime == null)
            throw new Exception();

        return new SnapshotHeader
        {
            SerializerId = serializerId.Value,
            OriginalPath = originalPath,
            CreationTime = creationTime.Value
        };
    }

    public IEnumerable<HFile> ReadFiles()
    {
        if (jSnapshotReader.CurrentPropertyType != JSnapshotFieldType.FileCollection)
            throw new Exception();

        IEnumerable<JFileReader> fileReaders = jSnapshotReader.ReadFiles();

        foreach (JFileReader jFileReader in fileReaders)
            yield return ReadFile(jFileReader);
    }

    private HFile ReadFile(JFileReader jFileReader)
    {
        HFile hFile = new();

        bool isFinished = false;
        while (!isFinished)
        {
            switch (jFileReader.CurrentPropertyType)
            {
                case JFileFieldType.None:
                    isFinished = true;
                    break;

                case JFileFieldType.FileName:
                    hFile.Name = jFileReader.ReadName();
                    break;

                case JFileFieldType.FileSize:
                    hFile.Size = jFileReader.ReadSize();
                    break;

                case JFileFieldType.LastModifiedTime:
                    hFile.LastModifiedTime = jFileReader.ReadLastModifiedTime();
                    break;

                case JFileFieldType.Hash:
                    hFile.Hash = jFileReader.ReadHash();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            jSnapshotReader.MoveNext();
        }

        return hFile;
    }

    public IEnumerable<IDirectoryReader> ReadDirectories()
    {
        throw new NotImplementedException();
    }
}