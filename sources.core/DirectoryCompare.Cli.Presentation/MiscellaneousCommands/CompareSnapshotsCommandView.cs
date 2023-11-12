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

using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.DirectoryCompare.Cli.Application.UseCases.MiscellaneousArea.CompareSnapshots;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.MiscellaneousCommands;

internal class CompareSnapshotsCommandView : IView<CompareSnapshotsCommand>
{
    public void Display(CompareSnapshotsCommand command)
    {
        DisplayOnlyInSnapshot1(command);
        
        DisplayOnlyInSnapshot2(command);

        DisplayDifferentNames(command);

        DisplayDifferentContent(command);

        Console.WriteLine();
        if (command.ExportDirectoryPath != null)
            CustomConsole.WriteLine("Results exported also into directory: {0}", command.ExportDirectoryPath);
    }

    private static void DisplayOnlyInSnapshot1(CompareSnapshotsCommand command)
    {
        DisplaySubtitle("Files only in snapshot 1:");
        
        foreach (string path in command.OnlyInSnapshot1)
            Console.WriteLine(path);
    }

    private static void DisplayOnlyInSnapshot2(CompareSnapshotsCommand command)
    {
        DisplaySubtitle("Files only in snapshot 2:");
        
        foreach (string path in command.OnlyInSnapshot2)
            Console.WriteLine(path);
    }

    private static void DisplayDifferentNames(CompareSnapshotsCommand command)
    {
        DisplaySubtitle("Different names:");
        
        bool isFirst = true;
        
        foreach (FilePairDto itemComparison in command.DifferentNames)
        {
            if (isFirst)
                isFirst = false;
            else
                Console.WriteLine();

            Console.WriteLine("1 - " + itemComparison.FullName1);
            Console.WriteLine("2 - " + itemComparison.FullName2);
        }
    }

    private static void DisplayDifferentContent(CompareSnapshotsCommand command)
    {
        DisplaySubtitle("Different content:");
        
        bool isFirst = true;
        
        foreach (FilePairDto itemComparison in command.DifferentContent)
        {
            if (isFirst)
                isFirst = false;
            else
                Console.WriteLine();

            Console.WriteLine("1 - " + itemComparison.FullName1);
            Console.WriteLine("2 - " + itemComparison.FullName2);
        }
    }

    private static void DisplaySubtitle(string text)
    {
        HorizontalLine horizontalLine1 = new()
        {
            Margin = "0 1 0 0"
        };
        horizontalLine1.Display();
        
        Console.WriteLine(text);
        
        HorizontalLine horizontalLine2 = new()
        {
            Margin = "0 0 0 1"
        };
        horizontalLine2.Display();
    }
}