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
using DustInTheWind.DirectoryCompare.Cli.Application.UseCases.SnapshotArea.PresentSnapshot;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.SnapshotCommands;

public class DisplaySnapshotCommandView : IView<DisplaySnapshotCommand>
{
    public void Display(DisplaySnapshotCommand command)
    {
        if (command.Response == null)
            CustomConsole.WriteLine("There is no snapshot.");
        else
        {
            Console.WriteLine($"Snapshot: {command.Response.SnapshotId:D}");
            Console.WriteLine($"Path: {command.Response.OriginalPath}");
            Console.WriteLine();

            DisplayDirectory(command.Response.RootDirectory, 0);
        }
    }

    private static void DisplayDirectory(DirectoryDto directory, int index)
    {
        string indent = new(' ', index * 2);

        foreach (DirectoryDto subdirectory in directory.Directories)
        {
            Console.WriteLine(indent + subdirectory.Name);
            DisplayDirectory(subdirectory, index + 1);
        }

        foreach (FileDto file in directory.Files)
        {
            Console.Write(indent + file.Name);
            CustomConsole.WriteLine(ConsoleColor.DarkGray, " [" + file.Hash + "]");
        }
    }
}