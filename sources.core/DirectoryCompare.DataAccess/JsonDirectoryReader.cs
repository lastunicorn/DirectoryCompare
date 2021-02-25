using System;
using System.Collections.Generic;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Domain.ImportExport;

namespace DustInTheWind.DirectoryCompare.DataAccess
{
    public class JsonDirectoryReader : IDirectoryReader
    {

        public DirectoryItemType CurrentItemType { get; }

        public JsonDirectoryReader()
        {

        }

        public bool MoveNext()
        {
            throw new NotImplementedException();
        }

        public string ReadName()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<HFile> ReadFiles()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IDirectoryReader> ReadDirectories()
        {
            throw new NotImplementedException();
        }
    }
}