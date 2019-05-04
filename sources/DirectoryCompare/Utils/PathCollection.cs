// DirectoryCompare
// Copyright (C) 2017 Dust in the Wind
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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace DustInTheWind.DirectoryCompare.Utils
{
    public class PathCollection : Collection<string>
    {
        public PathCollection()
        {
        }

        public PathCollection(IList<string> items)
            : base(items)
        {
        }

        public PathCollection ToAbsolutePaths(string rootPath)
        {
            string[] newItems = Items
                .Select(x => Path.IsPathRooted(x) ? x : Path.Combine(rootPath, x))
                .ToArray();

            return new PathCollection(newItems);
        }

        public void AddRange(IEnumerable<string> items)
        {
            foreach (string item in items)
                Items.Add(item);
        }
    }
}