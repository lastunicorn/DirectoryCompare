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

namespace DustInTheWind.DirectoryCompare.Cli.UI.ResultExporters
{
    internal class ConsoleComparisonExporter
    {
        public void Export(SnapshotComparer comparer)
        {
            DisplayResults(comparer);
        }

        public static void DisplayResults(SnapshotComparer comparer)
        {
            Console.WriteLine();

            Console.WriteLine("Files only in snapshot 1:");
            foreach (string path in comparer.OnlyInSnapshot1)
                Console.WriteLine(path);

            Console.WriteLine();

            Console.WriteLine("Files only in snapshot 2:");
            foreach (string path in comparer.OnlyInSnapshot2)
                Console.WriteLine(path);

            Console.WriteLine();

            Console.WriteLine("Different names:");
            foreach (ItemComparison itemComparison in comparer.DifferentNames)
            {
                Console.WriteLine("1 - " + itemComparison.FullName1);
                Console.WriteLine("2 - " + itemComparison.FullName2);
            }

            Console.WriteLine();

            Console.WriteLine("Different content:");
            foreach (ItemComparison itemComparison in comparer.DifferentContent)
            {
                Console.WriteLine("1 - " + itemComparison.FullName1);
                Console.WriteLine("2 - " + itemComparison.FullName2);
            }
        }
    }
}