namespace DustInTheWind.DirectoryCompare.Ports.LogAccess;

public class RemoveDuplicatesPlan
{
    public string SnapshotLeft { get; set; }

    public string SnapshotRight { get; set; }

    public string RemovePart { get; set; }

    public string PurgatoryDirectory { get; set; }
}