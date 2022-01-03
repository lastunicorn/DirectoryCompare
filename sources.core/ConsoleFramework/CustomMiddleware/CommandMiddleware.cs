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
        private readonly ICommandModelFactory commandModelFactory;
        private readonly ICommandViewFactory commandViewFactory;

        public CommandMiddleware(CommandPool commandPool, ICommandModelFactory commandModelFactory, ICommandViewFactory commandViewFactory)
        {
            this.commandPool = commandPool ?? throw new ArgumentNullException(nameof(commandPool));
            this.commandModelFactory = commandModelFactory ?? throw new ArgumentNullException(nameof(commandModelFactory));
            this.commandViewFactory = commandViewFactory ?? throw new ArgumentNullException(nameof(commandViewFactory));
        }

        public async Task InvokeAsync(ConsoleRequestContext context, RequestDelegate next)
        {
            CommandSeed commandSeed = commandPool.GetMatchingCommand(context.Arguments);

            ICommandModel commandModel = commandSeed.CreateModel(commandModelFactory);
            object commandView = commandSeed.CreateView(commandViewFactory);

            bool isLongCommandView = IsLongCommandView(commandView);
            if (isLongCommandView)
                await ExecuteLongCommand(commandModel, commandView, context);
            else
                await ExecuteCommand(commandModel, commandView, context);

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

        private static async Task ExecuteLongCommand(ICommandModel commandModel, object commandView, ConsoleRequestContext context)
        {
            if (commandView != null)
                ExecuteView(commandView, commandModel);

            await commandModel.Execute(context.Arguments);

            if (commandView != null)
                ExecuteFinishView(commandView, commandModel);
        }

        private static async Task ExecuteCommand(ICommandModel commandModel, object commandView, ConsoleRequestContext context)
        {
            await commandModel.Execute(context.Arguments);

            if (commandView != null)
                ExecuteView(commandView, commandModel);
        }

        private static void ExecuteView(object view, ICommandModel commandModel)
        {
            MethodInfo methodInfo = view.GetType().GetMethod(nameof(ICommandView<ICommandModel>.Display));
            methodInfo?.Invoke(view, new object[] { commandModel });
        }

        private static void ExecuteFinishView(object view, ICommandModel commandModel)
        {
            MethodInfo methodInfo = view.GetType().GetMethod(nameof(ILongCommandView<ICommandModel>.FinishDisplay));
            methodInfo?.Invoke(view, new object[] { commandModel });
        }
    }
}