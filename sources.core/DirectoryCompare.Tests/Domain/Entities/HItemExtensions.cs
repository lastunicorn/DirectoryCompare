// DirectoryCompare
// Copyright (C) 2017-2020 Dust in the Wind
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

using DustInTheWind.DirectoryCompare.Domain.Entities;

namespace DustInTheWind.DirectoryCompare.Tests.Domain.Entities;

internal static class HItemExtensions
{
    public static HItem CreateFile(string name)
    {
        return new HFile
        {
            Name = name
        };
    }

    public static HItem CreateDirectory(string name)
    {
        return new HDirectory
        {
            Name = name
        };
    }

    public static HItem PlaceInto(this HItem hItem, string parentName)
    {
        HDirectory parentDirectory = new HDirectory
        {
            Name = parentName
        };

        hItem.Parent = parentDirectory;

        return parentDirectory;
    }
}