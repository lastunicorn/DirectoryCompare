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
using DustInTheWind.DirectoryCompare.Cli.Application.SnapshotArea.PresentSnapshot;
using DustInTheWind.DirectoryCompare.DataStructures;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.SnapshotCommands.DisplaySnapshot;

public class DisplaySnapshotView : ViewBase<SnapshotViewModel>
{
    public override void Display(SnapshotViewModel snapshotViewModel)
    {
        DisplaySnapshotMetadata(snapshotViewModel);

        if (snapshotViewModel.RootDirectory != null)
            DisplaySnapshotContent(snapshotViewModel.DirectoryPath, snapshotViewModel.RootDirectory);
    }

    private static void DisplaySnapshotMetadata(SnapshotViewModel snapshotViewModel)
    {
        SnapshotDataGrid snapshotDataGrid = new();
        snapshotDataGrid.AddSnapshot(snapshotViewModel);

        snapshotDataGrid.Display();
    }

    private static void DisplaySnapshotContent(SnapshotPath directoryPath, DirectoryDto rootDirectory)
    {
        Console.WriteLine();

        CustomConsole.WriteLine(directoryPath);

        DirectoryView directoryView = new(rootDirectory);
        directoryView.Display();
    }
}