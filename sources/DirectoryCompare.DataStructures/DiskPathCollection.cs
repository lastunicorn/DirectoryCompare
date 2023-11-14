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

namespace DustInTheWind.DirectoryCompare.DataStructures;

public class DiskPathCollection : Collection<DiskPath>
{
    public DiskPathCollection()
    {
    }

    public DiskPathCollection(IList<DiskPath> items)
        : base(items)
    {
    }

    public DiskPathCollection(IEnumerable<DiskPath> items)
        : base(items.ToArray())
    {
    }

    public DiskPathCollection(IEnumerable<string> items)
        : base(items.Select(x => new DiskPath(x)).ToArray())
    {
    }

    /// <summary>
    /// Prepends the specified path to all items that are not already rooted
    /// and returns a new <see cref="DiskPathCollection"/> with the resulted items.
    /// </summary>
    public DiskPathCollection PrependPath(string path)
    {
        DiskPath[] newItems = Items
            .Select(x => x.Prepend(path))
            .ToArray();

        return new DiskPathCollection(newItems);
    }

    public void AddRange(IEnumerable<DiskPath> items)
    {
        foreach (DiskPath item in items)
            Items.Add(item);
    }

    public List<string> ToListOfStrings()
    {
        return Items
            .Select(x => (string)x)
            .ToList();
    }
}