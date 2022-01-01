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
using System.Reflection;

namespace DustInTheWind.ConsoleFramework
{
    public class CommandInfo
    {
        private readonly CommandAttribute commandAttribute;
        private readonly CommandDescriptionAttribute commandDescriptionAttribute;

        public bool IsValidCommand { get; }

        public Type Type { get; }

        public string Name
        {
            get
            {
                if (!IsValidCommand)
                    return null;

                if (commandAttribute != null)
                    return commandAttribute.Name;

                return Type.Name.EndsWith("Command")
                    ? Type.Name.Substring(0, Type.Name.Length - "Command".Length)
                    : Type.Name;
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

        public CommandInfo(Type type)
        {
            Type = type;
            IsValidCommand = CalculateIsValidCommand();

            if (!IsValidCommand)
                return;

            commandAttribute = Type?.GetCustomAttribute<CommandAttribute>();
            commandDescriptionAttribute = Type?.GetCustomAttribute<CommandDescriptionAttribute>();
        }

        private bool CalculateIsValidCommand()
        {
            return Type != null &&
                   Type.IsClass &&
                   !Type.IsAbstract &&
                   typeof(ICommand).IsAssignableFrom(Type);
        }
    }
}