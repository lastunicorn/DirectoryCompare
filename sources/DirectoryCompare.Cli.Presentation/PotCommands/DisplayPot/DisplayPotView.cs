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
using DustInTheWind.DirectoryCompare.DataStructures;

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
        WriteValue("Name", viewModel.Name);
        WriteValue("GUID", viewModel.Guid);
        WriteValue("Path", viewModel.Path);

        if (viewModel.IncludedPaths is { Count: > 0 })
        {
            CustomConsole.WriteLineEmphasized("Included Paths:");
            WithIndentation(() =>
            {
                foreach (SnapshotPath path in viewModel.IncludedPaths)
                    WriteInfo(path);
            });
        }

        WriteValue("Size", viewModel.Size.ToString("D"));

        if (viewModel.Description != null)
            WriteValue("Description", viewModel.Description);

        DisplaySnapshots(viewModel.Snapshots);
    }

    private void DisplaySnapshots(List<SnapshotViewModel> snapshots)
    {
        if (snapshots is { Count: > 0 })
        {
            CustomConsole.WriteLine();

            CustomConsole.WriteLineEmphasized($"Snapshots (Count = {snapshots.Count})");

            foreach (SnapshotViewModel snapshot in snapshots)
            {
                int index = snapshot.Index;
                DateTime creationTime = snapshot.CreationTime.ToLocalTime();
                CustomConsole.Write($"  [{index}] {creationTime}");

                CustomConsole.Write(ConsoleColor.DarkGray, $" ({snapshot.Size})");
                CustomConsole.WriteLine(ConsoleColor.DarkGray, $" {snapshot.Id:D}");
            }
        }
        else
        {
            WriteValue("Snapshots", "<none>");
        }
    }
}