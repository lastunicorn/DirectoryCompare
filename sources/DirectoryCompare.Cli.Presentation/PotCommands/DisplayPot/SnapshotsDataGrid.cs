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

using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.PotCommands.DisplayPot;

internal class SnapshotsDataGrid : CustomDataGrid
{
    public SnapshotsDataGrid()
    {
        CreateColumns();
    }

    private void CreateColumns()
    {
        Column nameColumn = new("Index")
        {
            CellHorizontalAlignment = ConsoleTools.Controls.HorizontalAlignment.Right
        };
        Columns.Add(nameColumn);

        Column valueColumn = new("Date");
        Columns.Add(valueColumn);

        Column sizeColumn = new("Size")
        {
            CellHorizontalAlignment = ConsoleTools.Controls.HorizontalAlignment.Right,
            ForegroundColor = ConsoleColor.DarkGray
        };
        Columns.Add(sizeColumn);

        Column guidColumn = new("Id")
        {
            ForegroundColor = ConsoleColor.DarkGray
        };
        Columns.Add(guidColumn);
    }

    public void AddSnapshots(IEnumerable<SnapshotViewModel> snapshots)
    {
        foreach (SnapshotViewModel snapshot in snapshots)
        {
            int index = snapshot.Index;
            DateTime creationTime = snapshot.CreationTime.ToLocalTime();
            string size = snapshot.Size.ToString();
            string guid = snapshot.Id.ToString("D");

            Rows.Add(index, creationTime, size, guid);
        }

        TitleRow.TitleCell.Content = $"Snapshots (Count = {Rows.Count})";
    }
}