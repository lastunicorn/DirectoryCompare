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

using System;
using System.Collections.ObjectModel;
using System.Linq;
using DirectoryCompare.CliFramework.Commands;

namespace DirectoryCompare.CliFramework
{
    public class CommandCollection : Collection<CommandCollectionItem>
    {
        public void Add(string key, ICommand command)
        {
            Add(new CommandCollectionItem(key, command));
        }

        protected override void InsertItem(int index, CommandCollectionItem item)
        {
            if (this.Any(x => x.Key == item.Key))
                throw new ArgumentException("There is another command with the same key.", nameof(item.Key));

            base.InsertItem(index, item);
        }

        public bool Contains(string commandKey)
        {
            return commandKey != null && this.Any(x => x.Key == commandKey);
        }

        public bool Contains(ICommand command)
        {
            return command != null && this.Any(x => x.Command == command);
        }

        public ICommand SelectCommand(Arguments arguments)
        {
            ICommand command;

            if (string.IsNullOrEmpty(arguments.Command))
            {
                command = Items
                    .Select(x => x.Command)
                    .OfType<HelpCommand>()
                    .FirstOrDefault();

                if (command == null)
                    throw new ConsoleFrameworkException("Please provide a command name to execute.");
            }
            else
            {
                if (!Contains(arguments.Command))
                    throw new ConsoleFrameworkException("Invalid command.");

                command = this[arguments.Command];
            }

            command.Initialize(arguments);

            return command;
        }

        public ICommand this[string commandKey]
        {
            get
            {
                if (commandKey == null)
                    return null;

                return Items
                    .Where(x => x.Key == commandKey)
                    .Select(x => x.Command)
                    .FirstOrDefault();
            }
        }
    }
}