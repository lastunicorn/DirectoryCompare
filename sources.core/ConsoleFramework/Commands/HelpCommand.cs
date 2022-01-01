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

using System;
using System.Collections.Generic;
using System.Linq;
using DustInTheWind.ConsoleFramework.UserControls;

namespace DustInTheWind.ConsoleFramework.Commands
{
    [CommandDescription("Displays information about the available commands.")]
    public class HelpCommand : ICommand
    {
        private readonly CommandInfoCollection commands;

        public HelpCommand(CommandInfoCollection commands)
        {
            this.commands = commands ?? throw new ArgumentNullException(nameof(commands));
        }

        public void Execute(Arguments arguments)
        {
            IEnumerable<IGrouping<CommandInfo, CommandInfo>> commandsGrouped = commands.GroupBy(x => x);

            UsageControl usageControl = new()
            {
                CommandNames = commandsGrouped
                    .Select(GetCommandNames)
                    .ToList()
            };

            usageControl.Display();
        }

        private static string GetCommandNames(IEnumerable<CommandInfo> group)
        {
            IEnumerable<string> commandNames = group.Select(x => x.Name);
            return string.Join(", ", commandNames);
        }
    }
}