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
using System.Linq;
using System.Reflection;
using DustInTheWind.ConsoleFramework.Commands;

namespace DustInTheWind.ConsoleFramework
{
    internal class CommandInfo
    {
        private readonly Type commandType;
        private readonly Type viewType;
        private readonly CommandAttribute commandAttribute;
        private readonly CommandDescriptionAttribute commandDescriptionAttribute;

        public bool IsValidCommand { get; }

        public string Name
        {
            get
            {
                if (!IsValidCommand)
                    return null;

                if (commandAttribute != null)
                    return commandAttribute.Name;

                return commandType.Name.EndsWith("Command")
                    ? commandType.Name.Substring(0, commandType.Name.Length - "Command".Length)
                    : commandType.Name;
            }
        }

        public string Description
        {
            get
            {
                if (!IsValidCommand)
                    return null;

                return commandDescriptionAttribute?.Description;
            }
        }

        public bool IsHelpCommand => commandAttribute?.IsHelp ?? false;

        public CommandInfo(Type commandType, Type viewType)
        {
            this.commandType = commandType ?? throw new ArgumentNullException(nameof(commandType));
            this.viewType = viewType;
            
            IsValidCommand = IsCommand(commandType);

            if (IsValidCommand)
            {
                commandAttribute = this.commandType?.GetCustomAttribute<CommandAttribute>();
                commandDescriptionAttribute = this.commandType?.GetCustomAttribute<CommandDescriptionAttribute>();
            }
        }

        public static bool IsCommand(Type type)
        {
            return type != null &&
                   type.IsClass &&
                   !type.IsAbstract &&
                   typeof(ICommand).IsAssignableFrom(type);
        }

        public static bool IsView(Type type)
        {
            return type != null &&
                   type.IsClass &&
                   !type.IsAbstract &&
                   type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IView<>));
        }

        public CommandSeed GenerateSeed(Arguments arguments = null)
        {
            return new CommandSeed(commandType, viewType, arguments);
        }

        public override string ToString()
        {
            return $"{Name} - {commandType.FullName}";
        }
    }
}