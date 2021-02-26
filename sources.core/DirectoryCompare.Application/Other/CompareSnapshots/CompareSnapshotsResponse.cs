using DustInTheWind.DirectoryCompare.Domain.Comparison;

namespace DustInTheWind.DirectoryCompare.Application.Other.CompareSnapshots
{
    public class CompareSnapshotsResponse
    {
        public SnapshotComparer SnapshotComparer { get; set; }

        public string ExportDirectoryPath { get; set; }
    }
}