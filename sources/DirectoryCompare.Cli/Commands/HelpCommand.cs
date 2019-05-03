// DirectoryCompare
// Copyright (C) 2019 Dust in the Wind
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

using System;
using System.Collections.Generic;
using System.Linq;

namespace DustInTheWind.DirectoryCompare.Cli.Commands
{
    internal class HelpCommand : ICommand
    {
        private readonly CommandCollection commandCollection;

        public HelpCommand(CommandCollection commandCollection)
        {
            this.commandCollection = commandCollection ?? throw new ArgumentNullException(nameof(commandCollection));
        }

        public void DisplayInfo()
        {
            Console.WriteLine("Displays information about the available commands");
        }

        public void Initialize(Arguments arguments)
        {
        }

        public void Execute()
        {
            IEnumerable<IGrouping<ICommand, KeyValuePair<string, ICommand>>> commandsGrouped = commandCollection.GroupBy(x => x.Value);

            foreach (IGrouping<ICommand, KeyValuePair<string, ICommand>> group in commandsGrouped)
            {
                string commandNames = GetCommandNames(group);
                Console.WriteLine(commandNames);
            }
        }

        private static string GetCommandNames(IEnumerable<KeyValuePair<string, ICommand>> group)
        {
            IEnumerable<string> commandNames = group.Select(x => x.Key);
            return string.Join(", ", commandNames);
        }
    }
}