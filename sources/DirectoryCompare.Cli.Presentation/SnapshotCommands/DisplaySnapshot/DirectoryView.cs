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
using DustInTheWind.DirectoryCompare.Cli.Application.SnapshotArea.PresentSnapshot;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.SnapshotCommands.DisplaySnapshot;

internal class DirectoryView
{
    private readonly DirectoryDto directory;
    private readonly GroupingLine groupingLine;

    public bool IsRoot { get; set; }

    public DirectoryView(DirectoryDto directory, GroupingLine parentGroupingLine = null)
    {
        this.directory = directory ?? throw new ArgumentNullException(nameof(directory));

        int itemCount = this.directory.Directories.Count + this.directory.Files.Count;
        groupingLine = new GroupingLine(itemCount, parentGroupingLine);
    }

    public void Display()
    {
        foreach (DirectoryDto subdirectory in directory.Directories)
        {
            DisplayDirectoryDetails(subdirectory);

            DirectoryView directoryView = new(subdirectory, groupingLine);
            directoryView.Display();
        }

        foreach (FileDto file in directory.Files)
            DisplayFileDetails(file);
    }

    private void DisplayDirectoryDetails(DirectoryDto subdirectory)
    {
        groupingLine.DisplayNext();
        Console.WriteLine($"[{subdirectory.Name}]");
    }

    private void DisplayFileDetails(FileDto file)
    {
        groupingLine.DisplayNext();

        Console.Write(file.Name + " ");

        if (file.Hash == null)
            CustomConsole.WriteLine(ConsoleColor.Magenta, "<null>");
        else
            CustomConsole.WriteLine(ConsoleColor.DarkGray, $"[{file.Hash}]");
    }
}