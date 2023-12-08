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

using System.Globalization;
using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Controls.Tables;
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
        DataGrid dataGrid = new()
        {
            HeaderRow =
            {
                IsVisible = false,
                ForegroundColor = ConsoleColor.White
            },
            Border =
            {
                ForegroundColor = ConsoleColor.DarkGray
            },
            TitleRow =
            {
                ForegroundColor = ConsoleColor.White,
                BackgroundColor = ConsoleColor.DarkGray,
                TitleCell =
                {
                    Content = $"{snapshotViewModel.PotName} > {snapshotViewModel.SnapshotId:D}"
                }
            }
        };

        Column nameColumn = new("Name")
        {
            ForegroundColor = ConsoleColor.White
        };
        dataGrid.Columns.Add(nameColumn);

        Column valueColumn = new("Value")
        {
            ForegroundColor = ConsoleColor.DarkGray
        };
        dataGrid.Columns.Add(valueColumn);

        dataGrid.Rows.Add("Path", snapshotViewModel.OriginalPath);
        dataGrid.Rows.Add("Creation Time", $"{snapshotViewModel.CreationTime.ToLocalTime()} ({CultureInfo.CurrentUICulture.Name})");
        dataGrid.Rows.Add("Directories", snapshotViewModel.TotalDirectoryCount.ToString("N0"));
        dataGrid.Rows.Add("Files", snapshotViewModel.TotalFileCount.ToString("N0"));

        string dataSize = snapshotViewModel.DataSize.ToString("D");
        dataGrid.Rows.Add("Data Size", dataSize);

        string storageSize = snapshotViewModel.StorageSize.ToString("D");
        dataGrid.Rows.Add("Snapshot Size", storageSize);

        dataGrid.Display();
    }

    private static void DisplaySnapshotContent(SnapshotPath directoryPath, DirectoryDto rootDirectory)
    {
        Console.WriteLine();

        CustomConsole.WriteLine(directoryPath);

        DirectoryView directoryView = new(rootDirectory);
        directoryView.Display();
    }
}