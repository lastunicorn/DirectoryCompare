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

using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.DirectoryCompare.Cli.Application.MiscellaneousArea.CompareSnapshots;
using DustInTheWind.DirectoryCompare.DataStructures;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.MiscellaneousCommands.CompareSnapshots;

internal class CompareSnapshotsCommandView : IView<CompareViewModel>
{
    public void Display(CompareViewModel compareViewModel)
    {
        DisplayOnlyInSnapshot1(compareViewModel);
        DisplayOnlyInSnapshot2(compareViewModel);
        DisplayDifferentNames(compareViewModel);
        DisplayDifferentContent(compareViewModel);

        Console.WriteLine();
        if (compareViewModel.ExportDirectoryPath != null)
            CustomConsole.WriteLine("Results exported also into directory: {0}", compareViewModel.ExportDirectoryPath);
    }

    private static void DisplayOnlyInSnapshot1(CompareViewModel compareViewModel)
    {
        DisplaySubtitle("Files only in snapshot 1:");

        foreach (string path in compareViewModel.OnlyInSnapshot1)
            Console.WriteLine(path);
    }

    private static void DisplayOnlyInSnapshot2(CompareViewModel compareViewModel)
    {
        DisplaySubtitle("Files only in snapshot 2:");

        foreach (string path in compareViewModel.OnlyInSnapshot2)
            Console.WriteLine(path);
    }

    private static void DisplayDifferentNames(CompareViewModel compareViewModel)
    {
        DisplaySubtitle("Different names:");

        bool isFirst = true;

        foreach (FilePairDto itemComparison in compareViewModel.DifferentNames)
        {
            if (isFirst)
                isFirst = false;
            else
                Console.WriteLine();

            Console.WriteLine("1 - " + itemComparison.FullName1);
            Console.WriteLine("2 - " + itemComparison.FullName2);
        }
    }

    private static void DisplayDifferentContent(CompareViewModel compareViewModel)
    {
        DisplaySubtitle("Different content:");

        bool isFirst = true;

        foreach (FilePairDto itemComparison in compareViewModel.DifferentContent)
        {
            if (isFirst)
                isFirst = false;
            else
                Console.WriteLine();

            CustomConsole.Write($"1 - {itemComparison.FullName1}");
            CustomConsole.WriteLine(ConsoleColor.DarkGray, $" ({itemComparison.Size1.ToString(DataSizeUnit.Byte)} - {itemComparison.Hash1})");

            CustomConsole.Write($"2 - {itemComparison.FullName2}");
            CustomConsole.WriteLine(ConsoleColor.DarkGray, $" ({itemComparison.Size2.ToString(DataSizeUnit.Byte)} - {itemComparison.Hash2})");
        }
    }

    private static void DisplaySubtitle(string text)
    {
        CustomConsole.WithForegroundColor(ConsoleColor.DarkYellow, () =>
        {
            HorizontalLine horizontalLine1 = new()
            {
                Margin = "0 1 0 0"
            };
            horizontalLine1.Display();

            CustomConsole.WriteLine(text);

            HorizontalLine horizontalLine2 = new()
            {
                Margin = "0 0 0 1"
            };
            horizontalLine2.Display();
        });
    }
}