using System;

namespace DustInTheWind.DirectoryCompare.Domain.ImportExport
{
    public class SnapshotHeader
    {
        public Guid SerializerId { get; set; }

        public string OriginalPath { get; set; }

        public DateTime CreationTime { get; set; }
    }
}