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

namespace DustInTheWind.DirectoryCompare.DataAccess.PotFiles.BlacklistFileModel;

public class PathCollection : Collection<string>
{
    public PathCollection()
    {
    }

    public PathCollection(IList<string> items)
        : base(items)
    {
    }

    /// <summary>
    /// Prepends the specified path to all items that are not already rooted
    /// and returns a new <see cref="PathCollection"/> with the resulted items.
    /// </summary>
    public PathCollection PrependPath(string path)
    {
        string[] newItems = Items
            .Select(x => Path.IsPathRooted(x) ? x : Path.Combine(path, x))
            .ToArray();

        return new PathCollection(newItems);
    }

    public void AddRange(IEnumerable<string> items)
    {
        foreach (string item in items)
            Items.Add(item);
    }
}