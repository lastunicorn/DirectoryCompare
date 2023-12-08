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
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.PotCommands.DisplayPot;

internal class DisplayPotView : ViewBase<DisplayPotViewModel>
{
    public override void Display(DisplayPotViewModel viewModel)
    {
        if (viewModel.Exists)
            DisplayPotInfo(viewModel);
        else
            CustomConsole.WriteWarning("The pot does not exist.");
    }

    private void DisplayPotInfo(DisplayPotViewModel viewModel)
    {
        DataGrid dataGrid = new()
        {
            HeaderRow =
            {
                IsVisible = false
            },
            TitleRow =
            {
                BackgroundColor = ConsoleColor.DarkGray,
                ForegroundColor = ConsoleColor.White,
                TitleCell =
                {
                    Content = viewModel.Name
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

        dataGrid.Rows.Add("Id", viewModel.Guid);

        List<string> lines = new()
        {
            viewModel.Path
        };

        if (viewModel.IncludedPaths is { Count: > 0 })
        {
            IEnumerable<string> includedPaths = viewModel.IncludedPaths
                .Select(x => $"  {x}");

            lines.AddRange(includedPaths);
        }

        MultilineText pathText = new(lines);
        dataGrid.Rows.Add((MultilineText)"Path", pathText);

        dataGrid.Rows.Add("Size", viewModel.Size.ToString("D"));

        if (viewModel.Description != null)
            dataGrid.Rows.Add("Description", viewModel.Description);

        dataGrid.Display();

        DisplaySnapshots(viewModel.Snapshots);
    }

    private void DisplaySnapshots(List<SnapshotViewModel> snapshots)
    {
        if (snapshots is { Count: > 0 })
        {
            CustomConsole.WriteLine();

            DataGrid dataGrid = new()
            {
                HeaderRow =
                {
                    ForegroundColor = ConsoleColor.White
                },
                TitleRow =
                {
                    BackgroundColor = ConsoleColor.DarkGray,
                    ForegroundColor = ConsoleColor.White,
                    TitleCell =
                    {
                        Content = $"Snapshots (Count = {snapshots.Count})"
                    }
                }
            };

            Column nameColumn = new("Index")
            {
                CellHorizontalAlignment = HorizontalAlignment.Right
            };
            dataGrid.Columns.Add(nameColumn);

            Column valueColumn = new("Date");
            dataGrid.Columns.Add(valueColumn);

            Column sizeColumn = new("Size")
            {
                CellHorizontalAlignment = HorizontalAlignment.Right,
                ForegroundColor = ConsoleColor.DarkGray
            };
            dataGrid.Columns.Add(sizeColumn);

            Column guidColumn = new("Id")
            {
                ForegroundColor = ConsoleColor.DarkGray
            };
            dataGrid.Columns.Add(guidColumn);

            foreach (SnapshotViewModel snapshot in snapshots)
            {
                int index = snapshot.Index;
                DateTime creationTime = snapshot.CreationTime.ToLocalTime();
                string size = snapshot.Size.ToString();
                string guid = snapshot.Id.ToString("D");

                dataGrid.Rows.Add(index, creationTime, size, guid);
            }

            dataGrid.Display();
        }
        else
        {
            WriteValue("Snapshots", "<none>");
        }
    }
}