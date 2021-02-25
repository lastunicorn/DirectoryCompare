using System;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Domain.Utils;

namespace DustInTheWind.DirectoryCompare.Tests.Serialization
{
    internal class JFileFields
    {
        public string FileName { get; set; }

        public DataSize? FileSize { get; set; }

        public DateTime? LastModifiedTime { get; set; }

        public FileHash? FileHash { get; set; }
    }
}