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

using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.PotCommands.DisplayPots;

internal class PotsDataGrid : CustomDataGrid
{
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
        IEnumerable<ContentRow> rows = potsViewModel.Pots
            .Select(pot =>
            {
                string guid = pot.Guid.ToString()[..8];
                string path = pot.Path;
                if (pot.HasPathFilters)
                    path += " [*]";
                return new ContentRow(guid, pot.Name, pot.Size, path);
            });
        Rows.AddRange(rows);

        FooterRow.FooterCell.Content = $"Total Size: {potsViewModel.TotalSize:D}";
    }
}