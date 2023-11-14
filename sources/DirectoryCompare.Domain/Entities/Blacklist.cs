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
using DustInTheWind.DirectoryCompare.DataStructures;

namespace DustInTheWind.DirectoryCompare.Domain.Entities;

public class BlackList : Collection<BlackPath>
{
    public BlackList(DiskPathCollection paths)
    {
        IEnumerable<BlackPath> blackPaths = paths.Select(x => new BlackPath(x));
        AddRange(blackPaths);
    }

    private void AddRange(IEnumerable<BlackPath> blackPaths)
    {
        foreach (BlackPath blackPath in blackPaths)
            Items.Add(blackPath);
    }

    public bool MatchPath(HItem item)
    {
        return Items.Any(x => x.Matches(item));
    }
}