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

namespace DustInTheWind.ConsoleFramework
{
    public class CommandInfoCollection : Collection<CommandInfo>
    {
        public CommandInfoCollection()
        {
        }

        public CommandInfoCollection(IList<CommandInfo> list)
            : base(list)
        {
        }

        protected override void InsertItem(int index, CommandInfo item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            if (Items.Any(x => x.Name == item.Name))
                throw new ArgumentException("There is another command with the same key.", nameof(item.Name));

            base.InsertItem(index, item);
        }

        public CommandInfo SelectCommand(string commandName)
        {
            if (string.IsNullOrEmpty(commandName))
                return GetHelpCommand();

            return Items
                .FirstOrDefault(x => string.Equals(x.Name, commandName, StringComparison.InvariantCultureIgnoreCase));
        }

        private CommandInfo GetHelpCommand()
        {
            CommandInfo command = Items
                .FirstOrDefault(x => x.IsHelpCommand);

            if (command != null)
                return command;

            command = Items
                .FirstOrDefault(x => string.Equals(x.Name, "help", StringComparison.InvariantCultureIgnoreCase));

            return command;
        }
    }
}