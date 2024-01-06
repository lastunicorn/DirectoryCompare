// DirectoryCompare
// Copyright (C) 2017-2024 Dust in the Wind
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

using DustInTheWind.DirectoryCompare.DataStructures;
using DustInTheWind.DirectoryCompare.Domain.Comparison;
using DustInTheWind.DirectoryCompare.Domain.Entities;

namespace DustInTheWind.DirectoryCompare.Cli.Application.MiscellaneousArea.CompareSnapshots;

public class FilePairDto
{
    public string FullName1 { get; }

    public DataSize Size1 { get; }

    public string Hash1 { get; }

    public string FullName2 { get; }

    public DataSize Size2 { get; }

    public string Hash2 { get; }

    public FilePairDto(ItemComparison itemComparison)
    {
        FullName1 = itemComparison.FullName1;

        if (itemComparison.Item1 is HFile hFile1)
        {
            Size1 = hFile1.Size;
            Hash1 = hFile1.Hash.ToString();
        }

        FullName2 = itemComparison.FullName2;

        if (itemComparison.Item2 is HFile hFile2)
        {
            Size2 = hFile2.Size;
            Hash2 = hFile2.Hash.ToString();
        }
    }
}