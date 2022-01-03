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
using System.Reflection;
using DustInTheWind.ConsoleFramework.CustomMiddleware;

namespace DustInTheWind.ConsoleFramework
{
    internal class CommandSeed
    {
        public Type CommandType { get; }

        public Type ViewType { get; }

        public List<CommandParameterSeed> ParametersSeeds { get; }

        public bool IsUsingAllArguments { get; }

        public CommandSeed(Type commandType, Type viewType, Arguments arguments = null)
        {
            CommandType = commandType ?? throw new ArgumentNullException(nameof(commandType));
            ViewType = viewType;

            ParametersSeeds = commandType.GetProperties()
                .Select(x => new CommandParameterSeed
                {
                    PropertyInfo = x,
                    Attribute = x.GetCustomAttribute<CommandParameterAttribute>()
                })
                .Where(x => x.Attribute != null)
                .Select(x =>
                {
                    Argument? argument = arguments?.GetArgumentFor(x.Attribute);
                    string rawValue = argument?.Value;

                    if (rawValue != null)
                        x.Value = Convert.ChangeType(rawValue, x.PropertyInfo.PropertyType);

                    return x;
                })
                .ToList();

            IsUsingAllArguments = arguments == null || ParametersSeeds.Count == arguments.Count;
        }

        public ICommand CreateCommand(ICommandFactory commandFactory)
        {
            ICommand command = commandFactory.Create(CommandType);

            if (command == null)
                throw new InvalidCommandException();

            foreach (CommandParameterSeed parameterInfo in ParametersSeeds)
                parameterInfo.SetPropertyValueOn(command);

            return command;
        }

        public object CreateView(IViewFactory viewFactory)
        {
            if (ViewType == null)
                return null;

            object view = viewFactory.Create(ViewType);

            if (view == null)
                throw new InvalidCommandException();

            return view;
        }
    }
}