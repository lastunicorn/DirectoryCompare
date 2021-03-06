﻿// DirectoryCompare
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

using System;
using DustInTheWind.DirectoryCompare.Domain;
using DustInTheWind.DirectoryCompare.Domain.Comparison;
using DustInTheWind.DirectoryCompare.Domain.Utils;

namespace DustInTheWind.DirectoryCompare.Cli.UI.Views
{
    internal class FindDuplicatesView
    {
        public void WriteDuplicate(FileDuplicate duplicate)
        {
            Console.WriteLine(duplicate.FullPathLeft);
            Console.WriteLine(duplicate.FullPathRight);

            Console.WriteLine($"{duplicate.Size} ({duplicate.Size.ToString(DataSizeUnit.Byte)})");
            Console.WriteLine();
        }

        public void WriteSummary(int duplicateCount, DataSize totalSize)
        {
            Console.WriteLine($"Total duplicates: {duplicateCount:n0} files");
            Console.WriteLine($"Total size: {totalSize} ({totalSize.ToString(DataSizeUnit.Byte)})");
            Console.WriteLine();
        }
    }
}