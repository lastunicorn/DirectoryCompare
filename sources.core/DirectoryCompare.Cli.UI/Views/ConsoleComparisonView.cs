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

using System;
using DustInTheWind.DirectoryCompare.Domain.Comparison;

namespace DustInTheWind.DirectoryCompare.Cli.UI.Views
{
    internal class ConsoleComparisonView
    {
        public SnapshotComparer Comparer { get; set; }

        public void Display()
        {
            Console.WriteLine();

            Console.WriteLine("Files only in snapshot 1:");
            foreach (string path in Comparer.OnlyInSnapshot1)
                Console.WriteLine(path);

            Console.WriteLine();

            Console.WriteLine("Files only in snapshot 2:");
            foreach (string path in Comparer.OnlyInSnapshot2)
                Console.WriteLine(path);

            Console.WriteLine();

            Console.WriteLine("Different names:");
            foreach (ItemComparison itemComparison in Comparer.DifferentNames)
            {
                Console.WriteLine("1 - " + itemComparison.FullName1);
                Console.WriteLine("2 - " + itemComparison.FullName2);
            }

            Console.WriteLine();

            Console.WriteLine("Different content:");
            foreach (ItemComparison itemComparison in Comparer.DifferentContent)
            {
                Console.WriteLine("1 - " + itemComparison.FullName1);
                Console.WriteLine("2 - " + itemComparison.FullName2);
            }
        }
    }
}