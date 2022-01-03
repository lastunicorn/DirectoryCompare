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

using DustInTheWind.ConsoleFramework;
using DustInTheWind.ConsoleTools;
using DustInTheWind.DirectoryCompare.Domain.Entities;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.PotCommands
{
    internal class DisplayPotCommandView : ICommandView<DisplayPotCommandModel>
    {
        public void Display(DisplayPotCommandModel commandModel)
        {
            if (commandModel.Pot == null)
                return;

            CustomConsole.Write("Name: ");
            CustomConsole.WriteLineEmphasies(commandModel.Pot.Name);

            string guid = commandModel.Pot.Guid.ToString();
            CustomConsole.Write("GUID: ");
            CustomConsole.WriteLineEmphasies(guid);

            CustomConsole.Write("Path: ");
            CustomConsole.WriteLineEmphasies(commandModel.Pot.Path);

            if (commandModel.Pot.Description != null)
            {
                CustomConsole.Write("Description: ");
                CustomConsole.WriteLineEmphasies(commandModel.Pot.Description);
            }

            if (commandModel.Pot.Snapshots != null && commandModel.Pot.Snapshots.Count > 0)
            {
                CustomConsole.WriteLine("Snapshots: ");

                foreach (Snapshot snapshot in commandModel.Pot.Snapshots)
                    CustomConsole.WriteLineEmphasies("  - " + snapshot.CreationTime);
            }
            else
            {
                CustomConsole.Write("Snapshots: ");
                CustomConsole.WriteLineEmphasies("<none>");
            }
        }
    }
}