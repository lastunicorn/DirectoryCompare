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

using DustInTheWind.ConsoleTools;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Domain.PotModel;

namespace DustInTheWind.DirectoryCompare.Cli.UI.Views
{
    internal class PotView
    {
        private readonly Pot pot;

        public PotView(Pot pot)
        {
            this.pot = pot;
        }

        public void Display()
        {
            if (pot == null)
                return;

            CustomConsole.Write("Name: ");
            CustomConsole.WriteLineEmphasies(pot.Name);

            string guid = pot.Guid.ToString();
            CustomConsole.Write("GUID: ");
            CustomConsole.WriteLineEmphasies(guid);

            CustomConsole.Write("Path: ");
            CustomConsole.WriteLineEmphasies(pot.Path);

            if (pot.Description != null)
            {
                CustomConsole.Write("Description: ");
                CustomConsole.WriteLineEmphasies(pot.Description);
            }

            if (pot.Snapshots != null && pot.Snapshots.Count > 0)
            {
                CustomConsole.WriteLine("Snapshots: ");

                foreach (Snapshot snapshot in pot.Snapshots)
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