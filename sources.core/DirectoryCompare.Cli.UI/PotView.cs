﻿// DirectoryCompare
// Copyright (C) 2017-2019 Dust in the Wind
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
using DustInTheWind.DirectoryCompare.Domain.PotModel;

namespace DustInTheWind.DirectoryCompare.Cli.UI
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

            string guid = pot.Guid.ToString().Substring(0, 8);
            CustomConsole.Write("GUID: ");
            CustomConsole.WriteLineEmphasies(guid);
            
            CustomConsole.Write("Name: ");
            CustomConsole.WriteLineEmphasies(pot.Name);
            
            CustomConsole.Write("Path: ");
            CustomConsole.WriteLineEmphasies(pot.Path);
        }
    }
}