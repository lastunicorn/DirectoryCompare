using DustInTheWind.DirectoryCompare.Domain.Comparison;

namespace DustInTheWind.DirectoryCompare.Cli.Application.MiscellaneousArea.CompareSnapshots;

public class CompareSnapshotsResponse
{
    public IReadOnlyList<string> OnlyInSnapshot1 { get; set; }

    public IReadOnlyList<string> OnlyInSnapshot2 { get; set; }

    public IReadOnlyList<ItemComparison> DifferentNames { get; set; }

    public IReadOnlyList<ItemComparison> DifferentContent { get; set; }

    public string ExportDirectoryPath { get; set; }
}