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
using System.Threading.Tasks;
using DustInTheWind.ConsoleFramework.AppBuilder;

namespace DustInTheWind.ConsoleFramework.CustomMiddleware
{
    internal class CommandMiddleware : IConsoleMiddleware
    {
        private readonly CommandPool commandPool;
        private readonly ICommandFactory commandFactory;
        private readonly IViewFactory viewFactory;

        public CommandMiddleware(CommandPool commandPool, ICommandFactory commandFactory, IViewFactory viewFactory)
        {
            this.commandPool = commandPool ?? throw new ArgumentNullException(nameof(commandPool));
            this.commandFactory = commandFactory ?? throw new ArgumentNullException(nameof(commandFactory));
            this.viewFactory = viewFactory ?? throw new ArgumentNullException(nameof(viewFactory));
        }

        public async Task InvokeAsync(ConsoleRequestContext context, RequestDelegate next)
        {
            CommandSeed commandSeed = commandPool.GetMatchingCommand(context.Arguments);

            ICommand command = commandSeed.CreateCommand(commandFactory);
            object view = commandSeed.CreateView(viewFactory);

            bool isLongCommandView = IsLongCommandView(view);

            if (view != null && isLongCommandView)
            {
                ExecuteView(view, command);
            }

            await command.Execute(context.Arguments);

            if (view != null)
            {
                if (isLongCommandView)
                    ExecuteFinishView(view);
                else
                    ExecuteView(view, command);
            }

            if (next != null)
                await next.Invoke(context);
        }

        private static bool IsLongCommandView(object view)
        {
            if (view == null)
                return false;

            return view.GetType().GetInterfaces()
                .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ILongCommandView<>));
        }

        private static void ExecuteView(object view, ICommand command)
        {
            MethodInfo displayMethodInfo = view.GetType().GetMethod(nameof(IView<ICommand>.Display));
            displayMethodInfo?.Invoke(view, new object[] { command });
        }

        private static void ExecuteFinishView(object view)
        {
            MethodInfo displayMethodInfo = view.GetType().GetMethod(nameof(ILongCommandView<ICommand>.FinishDisplay));
            displayMethodInfo?.Invoke(view, Array.Empty<object>());
        }
    }
}