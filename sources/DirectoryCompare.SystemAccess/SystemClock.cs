using DustInTheWind.DirectoryCompare.Ports.SystemAccess;

namespace DustInTheWind.DirectoryCompare.LogAccess;

public class SystemClock : ISystemClock
{
    public DateTime GetCurrentUtcTime()
    {
        return DateTime.UtcNow;
    }
}