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
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DustInTheWind.ConsoleFramework.CustomMiddleware;

namespace DustInTheWind.ConsoleFramework
{
    internal class CommandPool : IEnumerable<CommandInfo>
    {
        private readonly ConcurrentBag<CommandInfo> items = new();

        public CommandPool()
        {
        }

        public CommandPool(IEnumerable<CommandInfo> list)
        {
            foreach (CommandInfo commandInfo in list)
                items.Add(commandInfo);
        }

        public CommandSeed GetMatchingCommand(Arguments arguments)
        {
            if (string.IsNullOrEmpty(arguments.Command))
                return GetHelpCommand().GenerateSeed();

            CommandSeed[] commandSeeds = items
                .Where(x => string.Equals(x.Name, arguments.Command, StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x.GenerateSeed(arguments))
                .Where(x => x.ParametersSeeds
                    .All(z => z.Attribute.Optional || z.Value != null))
                .OrderByDescending(x => x.ParametersSeeds.Count)
                .ToArray();

            switch (commandSeeds.Length)
            {
                // - if one command is a match (perfect or partial) => execute it
                case 1:
                    return commandSeeds[0];

                // - if more than one command is a match:
                //      - if only one perfect match => execute it
                //      - else => throw
                case > 1:
                    CommandSeed[] perfectMatches = commandSeeds
                        .Where(x => x.IsUsingAllArguments)
                        .ToArray();

                    if (perfectMatches.Length == 1)
                        return perfectMatches[0];

                    CommandSeed firstCommandSeed = commandSeeds[0];
                    CommandSeed secondCommandSeed = commandSeeds[1];

                    if (firstCommandSeed.ParametersSeeds.Count > secondCommandSeed.ParametersSeeds.Count)
                        return firstCommandSeed;

                    throw new Exception("Multiple commands were matched. Please refine the parameters.");

                // - if no match => throw
                case 0:
                    throw new InvalidCommandException();
            }

            throw new InvalidCommandException();
        }

        private CommandInfo GetHelpCommand()
        {
            CommandInfo command = items
                .FirstOrDefault(x => x.IsHelpCommand);

            if (command != null)
                return command;

            command = items
                .FirstOrDefault(x => string.Equals(x.Name, "help", StringComparison.InvariantCultureIgnoreCase));

            return command;
        }

        public IEnumerator<CommandInfo> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    // anonymous parameters
    //      - identified by index (start from 1)
    //      - the index count excludes the named parameters
    //
    // named parameters
    //      - identified by name (short and long)
    //
    // switch parameters
    //      - identified by name (short and long)
    //      - boolean value
    //      - no explicit value (default value = false)

    // params markers: / -
    // key-value separator: [space] = : 
}