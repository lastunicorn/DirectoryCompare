﻿using System;
using System.Collections.Generic;
using System.IO;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Domain.ImportExport;
using DustInTheWind.DirectoryCompare.JFiles.SnapshotFileModel;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.DataAccess
{
    public class JsonSnapshotReader : ISnapshotReader
    {
        private readonly Stack<JDirectoryWriter> directoryStack = new Stack<JDirectoryWriter>();
        private JSnapshotReader jSnapshotReader;

        public Guid Id => new Guid("9E93055D-7BDE-4F55-B340-DD5A4880D96E");

        public JsonSnapshotReader(TextReader textReader)
        {
            JsonTextReader jsonTextReader = new JsonTextReader(textReader);
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
            HFile hFile = new HFile();

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
}