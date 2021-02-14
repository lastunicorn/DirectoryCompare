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
using System.Collections.ObjectModel;
using System.Linq;
using DustInTheWind.ConsoleFramework.Commands;

namespace DustInTheWind.ConsoleFramework
{
    public class CommandCollection : Collection<ICommand>
    {
        public CommandCollection()
        {
        }

        public CommandCollection(IList<ICommand> list)
            : base(list)
        {
        }

        protected override void InsertItem(int index, ICommand item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            if (Items.Any(x => x.Key == item.Key))
                throw new ArgumentException("There is another command with the same key.", nameof(item.Key));

            base.InsertItem(index, item);
        }

        public bool Contains(string commandKey)
        {
            return commandKey != null && Items.Any(x => x.Key == commandKey);
        }

        public ICommand SelectCommand(string commandName)
        {
            ICommand command;

            if (string.IsNullOrEmpty(commandName))
            {
                command = Items
                    .OfType<HelpCommand>()
                    .FirstOrDefault();

                if (command == null)
                    throw new ConsoleFrameworkException("Please provide a command name to execute.");
            }
            else
            {
                if (!Contains(commandName))
                    throw new ConsoleFrameworkException("Invalid command.");

                command = this[commandName];
            }

            return command;
        }

        public ICommand this[string commandKey]
        {
            get
            {
                if (commandKey == null)
                    return null;

                return Items
                    .FirstOrDefault(x => x.Key == commandKey);
            }
        }
    }
}