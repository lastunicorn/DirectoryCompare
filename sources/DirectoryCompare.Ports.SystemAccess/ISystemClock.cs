namespace DustInTheWind.DirectoryCompare.Ports.SystemAccess;

public interface ISystemClock
{
    DateTime GetCurrentUtcTime();
}