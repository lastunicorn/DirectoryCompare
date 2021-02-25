using System.Collections.Generic;
using DustInTheWind.DirectoryCompare.Domain.Entities;

namespace DustInTheWind.DirectoryCompare.Domain.ImportExport
{
    public interface IDirectoryReader
    {
        string ReadName();

        IEnumerable<HFile> ReadFiles();

        IEnumerable<IDirectoryReader> ReadDirectories();
    }
}