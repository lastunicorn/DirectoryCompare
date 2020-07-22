// DirectoryCompare
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

using System.Collections.Generic;
using DustInTheWind.ConsoleTools;
using DustInTheWind.DirectoryCompare.Domain.PotModel;

namespace DustInTheWind.DirectoryCompare.Cli.UI
{
    internal class PotView
    {
        private readonly List<Pot> pots;

        public PotView(List<Pot> pots)
        {
            this.pots = pots;
        }

        public void Display()
        {
            if (pots == null)
                return;

            foreach (Pot pot in pots)
            {
                string guid = pot.Guid.ToString().Substring(0, 8);
                CustomConsole.Write(guid);
                CustomConsole.Write(" ");

                CustomConsole.WriteEmphasies(pot.Name);
                CustomConsole.Write(" - ");

                CustomConsole.WriteEmphasies(pot.Path);
                CustomConsole.WriteLine();

                //TextBlockBuilder textBlock = new TextBlockBuilder
                //{
                //    TextBlock.FromText(guid),
                //    " ",
                //    TextBlock.FromText(pot.Name, Emphasized),
                //    " - ",
                //    TextBlock.FromText(pot.Path),
                //};
            }
        }
    }
}