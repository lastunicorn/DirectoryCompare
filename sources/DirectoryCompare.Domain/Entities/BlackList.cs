using System.Collections.ObjectModel;

namespace DustInTheWind.DirectoryCompare.Domain.Entities;

public class BlackList : Collection<IBlackItem>
{
    public BlackList(IEnumerable<IBlackItem> items)
    {
        foreach (IBlackItem item in items)
            Items.Add(item);
    }

    public bool Match(HItem hItem)
    {
        return Items.Any(x => x.Match(hItem));
    }
}