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

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.PotCommands.DisplayPot;

internal class DisplayPotCommandView : ViewBase<DisplayPotViewModel>
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

        string guid = viewModel.Guid.ToString();
        WriteValue("GUID", viewModel.Guid);

        WriteValue("Path", viewModel.Path);

        if (viewModel.Description != null)
            WriteValue("Description", viewModel.Description);

        if (viewModel.Snapshots is { Count: > 0 })
        {
            CustomConsole.WriteLineEmphasized($"Snapshots (Count = {viewModel.Snapshots.Count})");
            DisplaySnapshots(viewModel.Snapshots);
        }
        else
        {
            WriteValue("Snapshots", "<none>");
        }
    }

    private static void DisplaySnapshots(List<SnapshotViewModel> snapshots)
    {
        foreach (SnapshotViewModel snapshot in snapshots)
        {
            int index = snapshot.Index;
            DateTime creationTime = snapshot.CreationTime.ToLocalTime();
            CustomConsole.Write($"  [{index}] {creationTime} - ");

            Guid id = snapshot.Id;
            CustomConsole.WriteLine(ConsoleColor.DarkGray, id.ToString("D"));
        }
    }
}