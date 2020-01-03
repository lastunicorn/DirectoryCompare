// DirectoryCompare
// Copyright (C) 2017-2019 Dust in the Wind
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
using DustInTheWind.DirectoryCompare.Domain.Comparison;
using DustInTheWind.DirectoryCompare.Domain.SomeInterfaces;

namespace DustInTheWind.DirectoryCompare.Cli.UI.ResultExporters
{
    internal class ConsoleDuplicatesExporter : IDuplicatesExporter
    {
        public void WriteDuplicate(FileDuplicate duplicate)
        {
            Console.WriteLine(duplicate.FullPath1);
            Console.WriteLine(duplicate.FullPath2);

            Console.WriteLine($"{duplicate.Size:n0} bytes");
            Console.WriteLine();
        }

        public void WriteSummary(int duplicateCount, long totalSize)
        {
            Console.WriteLine($"Total duplicates: {duplicateCount:n0} files");
            Console.WriteLine($"Total size: {totalSize:n0} bytes");
            Console.WriteLine();
        }
    }
}