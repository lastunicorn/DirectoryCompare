// DirectoryCompare
// Copyright (C) 2017-2023 Dust in the Wind
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Collections.ObjectModel;
using DustInTheWind.Clindy.Applications.PresentDuplicates;
using DustInTheWind.DirectoryCompare.DataStructures;

namespace DustInTheWind.Clindy.Applications;

public class DuplicateGroupCollection : Collection<DuplicateGroup>
{
    public DuplicateGroupCollection(IEnumerable<DuplicateGroup> items)
    {
        foreach (DuplicateGroup item in items)
        {
            AddInternal(item);
            Items.Add(item);
        }
    }

    public int TotalDuplicatesCount { get; private set; }

    public DataSize TotalSize { get; private set; } = DataSize.Zero;

    public void AddRange(IEnumerable<DuplicateGroup> items)
    {
        foreach (DuplicateGroup item in items)
        {
            AddInternal(item);
            Items.Add(item);
        }
    }

    protected override void InsertItem(int index, DuplicateGroup item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));

        AddInternal(item);
        base.InsertItem(index, item);
    }

    protected override void SetItem(int index, DuplicateGroup item)
    {
        DuplicateGroup removedItem = Items[index];
        RemoveInternal(removedItem);
        AddInternal(item);

        base.SetItem(index, item);
    }

    protected override void RemoveItem(int index)
    {
        DuplicateGroup removedItem = Items[index];
        RemoveInternal(removedItem);

        base.RemoveItem(index);
    }

    protected override void ClearItems()
    {
        TotalDuplicatesCount = 0;
        TotalSize = DataSize.Zero;

        base.ClearItems();
    }

    private void AddInternal(DuplicateGroup item)
    {
        int fileCount = item.FilePaths.Count;
        int duplicatesCount = ComputeDuplicatesCount(fileCount);

        TotalDuplicatesCount += duplicatesCount;
        TotalSize += duplicatesCount * item.FileSize;
    }

    private void RemoveInternal(DuplicateGroup item)
    {
        int fileCount = item.FilePaths.Count;
        int duplicatesCount = ComputeDuplicatesCount(fileCount);

        TotalDuplicatesCount -= duplicatesCount;
        TotalSize -= duplicatesCount * item.FileSize;
    }

    public IEnumerable<DuplicateGroup> EnumerateOrdered(bool ascending = true)
    {
        if (ascending)
        {
            return Items
                .OrderBy(x => x.FileSize);
        }

        return Items
            .OrderByDescending(x => x.FileSize);
    }

    private static int ComputeDuplicatesCount(int fileCount)
    {
        int value = 0;

        for (int i = 1; i < fileCount; i++)
            value += i;

        return value;
    }
}