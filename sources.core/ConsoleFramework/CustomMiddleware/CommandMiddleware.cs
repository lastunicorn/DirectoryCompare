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
using System.Threading.Tasks;
using DustInTheWind.ConsoleFramework.AppBuilder;

namespace DustInTheWind.ConsoleFramework.CustomMiddleware
{
    internal class CommandMiddleware : IConsoleMiddleware
    {
        private readonly CommandInfoCollection commands;
        private readonly ICommandFactory commandFactory;

        public CommandMiddleware(CommandInfoCollection commands, ICommandFactory commandFactory)
        {
            this.commands = commands ?? throw new ArgumentNullException(nameof(commands));
            this.commandFactory = commandFactory ?? throw new ArgumentNullException(nameof(commandFactory));
        }

        public async Task InvokeAsync(ConsoleRequestContext context, RequestDelegate next)
        {
            CommandInfo commandInfo = commands.SelectCommand(context.Arguments.Command);

            if (commandInfo == null)
                throw new ConsoleFrameworkException("Invalid command.");

            ICommand command = commandFactory.Create(commandInfo.Type);
            
            // todo: using reflection, inject parameters into the command.
            
            command.Execute(context.Arguments);

            if (next != null)
                await next.Invoke(context);
        }
    }
}