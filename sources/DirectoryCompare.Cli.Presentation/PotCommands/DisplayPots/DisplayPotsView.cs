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

using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.PotCommands.DisplayPots;

internal class DisplayPotsView : ViewBase<PotsViewModel>
{
    public override void Display(PotsViewModel potsViewModel)
    {
        bool hasPots = potsViewModel.Pots is { Count: > 0 };

        if (hasPots)
            DisplayPots(potsViewModel);
        else
            WriteInfo("There are no Pots.");
    }

    private static void DisplayPots(PotsViewModel potsViewModel)
    {
        DataGrid dataGrid = new()
        {
            HeaderRow =
            {
                ForegroundColor = ConsoleColor.White
            },
            Border =
            {
                ForegroundColor = ConsoleColor.DarkGray
            },
            TitleRow =
            {
                BackgroundColor = ConsoleColor.DarkGray,
                ForegroundColor = ConsoleColor.White,
                TitleCell =
                {
                    Content = "Pots"
                }
            }
        };

        dataGrid.Columns.Add("Id");
        
        Column nameColumn = new("Name")
        {
            ForegroundColor = ConsoleColor.White
        };
        dataGrid.Columns.Add(nameColumn);

        Column sizeColumn = new("Size", HorizontalAlignment.Right)
        {
            ForegroundColor = ConsoleColor.DarkGray
        };
        dataGrid.Columns.Add(sizeColumn);

        Column pathColumn = new("Path")
        {
            ForegroundColor = ConsoleColor.DarkGray
        };
        dataGrid.Columns.Add(pathColumn);

        IEnumerable<ContentRow> rows = potsViewModel.Pots
            .Select(pot =>
            {
                string guid = pot.Guid.ToString()[..8];
                string path = pot.Path;
                if (pot.HasPathFilters)
                    path += " [*]";
                return new ContentRow(guid, pot.Name, pot.Size, path);
            });

        dataGrid.Rows.AddRange(rows);

        dataGrid.FooterRow.FooterCell.Content = $"Total Size: {potsViewModel.TotalSize:D}";

        dataGrid.Display();
    }
}