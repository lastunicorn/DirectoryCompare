namespace DustInTheWind.DirectoryCompare.Domain.Entities;

public interface IBlackItem
{
    bool Match(HItem item);
}