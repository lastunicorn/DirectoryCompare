using System;
using System.Collections.Generic;
using DustInTheWind.DirectoryCompare.Domain.Entities;

namespace DustInTheWind.DirectoryCompare.Domain.ImportExport
{
    public interface ISnapshotReader
    {
        SnapshotItemType CurrentItemType { get; }

        bool MoveNext();

        SnapshotHeader ReadHeader();

        IEnumerable<HFile> ReadFiles();

        IEnumerable<IDirectoryReader> ReadDirectories();
    }
}