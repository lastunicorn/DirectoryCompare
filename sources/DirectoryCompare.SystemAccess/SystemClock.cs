using DustInTheWind.DirectoryCompare.Ports.SystemAccess;

namespace DustInTheWind.DirectoryCompare.SystemAccess;

public class SystemClock : ISystemClock
{
    public DateTime GetCurrentUtcTime()
    {
        return DateTime.UtcNow;
    }
}