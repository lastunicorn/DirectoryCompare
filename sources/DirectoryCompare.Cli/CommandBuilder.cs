// DirectoryCompare
// Copyright (C) 2017 Dust in the Wind
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

using DustInTheWind.DirectoryCompare.Cli.Commands;
using System;
using System.Linq;

namespace DustInTheWind.DirectoryCompare.Cli
{
    internal static class CommandBuilder
    {
        private static ICommand[] Commands = new ICommand[]
        {
            new ReadDiskCommand(),
            new ReadFileCommand(),
            new VerifyDiskCommand(),
            new CompareDisksCommand(),
            new CompareFilesCommand(),
            new FindDuplicatesCommand(),
            new RemoveDuplicatesCommand()
        };

        public static ICommand CreateCommand(Arguments arguments)
        {
            if (string.IsNullOrEmpty(arguments.Command))
                throw new Exception("Please provide a command name to execute.");

            ICommand command = Commands
                 .FirstOrDefault(x => x.Name == arguments.Command);

            if (command == null)
                throw new Exception("Invalid command.");

            command.Initialize(arguments);

            return command;
    }
}