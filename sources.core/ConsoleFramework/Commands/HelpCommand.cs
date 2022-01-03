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
using System.Threading.Tasks;

namespace DustInTheWind.ConsoleFramework.Commands
{
    [CommandDescription("Displays information about the available commands.")]
    internal class HelpCommand : ICommand
    {
        private readonly CommandPool commands;

        public IList<CommandViewModel> Commands { get; private set; }
        
        public HelpCommand(CommandPool commands)
        {
            this.commands = commands ?? throw new ArgumentNullException(nameof(commands));
        }

        public Task Execute(Arguments arguments)
        {
            Commands = commands
                .Select(x => new CommandViewModel
                {
                    Name = x.Name,
                    Description = x.Description
                })
                .ToList();

            return Task.CompletedTask;
        }
    }
}