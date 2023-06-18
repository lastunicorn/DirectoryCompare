// DirectoryCompare
// Copyright (C) 2017-2020 Dust in the Wind
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
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Domain.PotModel;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.PotCommands;

internal class DisplayPotCommandView : ViewBase<DisplayPotCommand>
{
    public override void Display(DisplayPotCommand command)
    {
        if (command.Pot == null)
            CustomConsole.WriteWarning("The pot does not exist.");
        else
            DisplayPotInfo(command.Pot);
    }

    private void DisplayPotInfo(Pot pot)
    {
        WriteValue("Name", pot.Name);

        string guid = pot.Guid.ToString();
        WriteValue("GUID", pot.Name);

        WriteValue("Path", pot.Path);

        if (pot.Description != null)
            WriteValue("Description", pot.Description);

        if (pot.Snapshots is { Count: > 0 })
        {
            WithIndentation("Snapshots:", () =>
            {
                DisplaySnapshots(pot.Snapshots);
            });
        }
        else
        {
            WriteValue("Snapshots", "<none>");
        }
    }

    private void DisplaySnapshots(List<Snapshot> snapshots)
    {
        foreach (Snapshot snapshot in snapshots)
        {
            DateTime creationTime = snapshot.CreationTime;
            Guid id = snapshot.Id;

            WriteValue(creationTime.ToString(CultureInfo.CurrentUICulture), id.ToString("D"));
        }
    }
}