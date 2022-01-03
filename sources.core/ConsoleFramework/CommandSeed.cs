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

        public ICommandModel CreateModel(ICommandModelFactory commandModelFactory)
        {
            ICommandModel commandModel = commandModelFactory.Create(CommandType);

            if (commandModel == null)
                throw new InvalidCommandException();

            foreach (CommandParameterSeed parameterInfo in ParametersSeeds)
                parameterInfo.SetPropertyValueOn(commandModel);

            return commandModel;
        }

        public object CreateView(ICommandViewFactory commandViewFactory)
        {
            if (ViewType == null)
                return null;

            object view = commandViewFactory.Create(ViewType);

            if (view == null)
                throw new InvalidCommandException();

            return view;
        }
    }
}