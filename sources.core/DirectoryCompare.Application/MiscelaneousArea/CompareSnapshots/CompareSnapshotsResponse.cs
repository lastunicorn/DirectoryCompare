using DustInTheWind.DirectoryCompare.Domain.Comparison;

namespace DustInTheWind.DirectoryCompare.Application.MiscelaneousArea.CompareSnapshots
{
    public class CompareSnapshotsResponse
    {
        public SnapshotComparer SnapshotComparer { get; set; }

        public string ExportDirectoryPath { get; set; }
    }
}