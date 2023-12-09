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
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.SnapshotCommands.DisplaySnapshot;

internal class SnapshotDataGrid : CustomDataGrid
{
    public SnapshotDataGrid()
    {
        HeaderRow.IsVisible = false;

        CreateColumns();
    }

    private void CreateColumns()
    {
        Column nameColumn = new("Name")
        {
            ForegroundColor = ConsoleColor.White
        };
        Columns.Add(nameColumn);

        Column valueColumn = new("Value")
        {
            ForegroundColor = ConsoleColor.DarkGray
        };
        Columns.Add(valueColumn);
    }

    public void AddSnapshot(SnapshotViewModel snapshotViewModel)
    {
        TitleRow.TitleCell.Content = $"{snapshotViewModel.PotName} > {snapshotViewModel.SnapshotId:D}";

        Rows.Add("Path", snapshotViewModel.OriginalPath);
        Rows.Add("Creation Time", $"{snapshotViewModel.CreationTime.ToLocalTime()} ({CultureInfo.CurrentUICulture.Name})");
        Rows.Add("Directories", snapshotViewModel.TotalDirectoryCount.ToString("N0"));
        Rows.Add("Files", snapshotViewModel.TotalFileCount.ToString("N0"));

        string dataSize = snapshotViewModel.DataSize.ToString("D");
        Rows.Add("Data Size", dataSize);

        string storageSize = snapshotViewModel.StorageSize.ToString("D");
        Rows.Add("Snapshot Size", storageSize);
    }
}