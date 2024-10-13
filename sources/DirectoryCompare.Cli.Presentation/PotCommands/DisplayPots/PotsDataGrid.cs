// Directory Compare
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

using DustInTheWind.ConsoleTools.Controls.Tables;
using DustInTheWind.DirectoryCompare.Cli.Presentation.Utils;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.PotCommands.DisplayPots;

internal class PotsDataGrid : CustomDataGrid
{
    public DataSizeFormat DataSizeFormat { get; set; }

    public PotsDataGrid()
    {
        TitleRow.TitleCell.Content = "Pots";

        CreateColumns();
    }

    private void CreateColumns()
    {
        Columns.Add("Id");

        Column nameColumn = new("Name")
        {
            ForegroundColor = ConsoleColor.White
        };
        Columns.Add(nameColumn);

        Column sizeColumn = new("Size", ConsoleTools.Controls.HorizontalAlignment.Right)
        {
            ForegroundColor = ConsoleColor.DarkGray
        };
        Columns.Add(sizeColumn);

        Column pathColumn = new("Path")
        {
            ForegroundColor = ConsoleColor.DarkGray
        };
        Columns.Add(pathColumn);
    }

    public void AddPots(PotsViewModel potsViewModel)
    {
        Console.WriteLine(DataSizeFormat);

        IEnumerable<ContentRow> rows = potsViewModel.Pots
            .Select(pot =>
            {
                string guid = pot.Guid.ToString()[..8];
                DataSizeDisplay size = pot.Size.ToDataSizeDisplay(DataSizeFormat);
                string path = pot.Path;
                if (pot.HasPathFilters)
                    path += " [*]";
                return new ContentRow(guid, pot.Name, size, path);
            });
        Rows.AddRange(rows);

        DataSizeDisplay totalSize = potsViewModel.TotalSize.ToDataSizeDisplay(DataSizeFormat | DataSizeFormat.Detailed);
        FooterRow.FooterCell.Content = $"Total Size: {totalSize}";
    }
}