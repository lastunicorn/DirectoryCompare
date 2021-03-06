﻿// DirectoryCompare
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
    public class HelpCommand : ICommand
    {
        private readonly CommandCollection commandCollection;

        public string Key { get; } = "help";

        public string Description => "Displays information about the available commands.";

        public HelpCommand(CommandCollection commandCollection)
        {
            this.commandCollection = commandCollection ?? throw new ArgumentNullException(nameof(commandCollection));
        }

        public void Execute(Arguments arguments)
        {
            IEnumerable<IGrouping<ICommand, ICommand>> commandsGrouped = commandCollection.GroupBy(x => x);

            UsageControl usageControl = new UsageControl
            {
                CommandNames = commandsGrouped
                    .Select(GetCommandNames)
                    .ToList()
            };

            usageControl.Display();
        }

        private static string GetCommandNames(IEnumerable<ICommand> group)
        {
            IEnumerable<string> commandNames = group.Select(x => x.Key);
            return string.Join(", ", commandNames);
        }
    }
}