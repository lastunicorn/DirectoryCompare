namespace DustInTheWind.DirectoryCompare.Ports.UserAccess;

public class StartNewSnapshotInfo
{
    public string PotName { get; set; }

    public string Path { get; set; }

    public List<string> BlackList { get; set; }

    public DateTime StartTime { get; set; }
}