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
using DustInTheWind.ConsoleFramework.AppBuilder;
using DustInTheWind.ConsoleFramework.Commands;
using DustInTheWind.ConsoleFramework.CustomMiddleware;
using DustInTheWind.ConsoleFramework.UserControls;
using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Spinners;

namespace DustInTheWind.ConsoleFramework
{
    public class ConsoleApplicationBase
    {
        private ApplicationHeader applicationHeader;
        private CommandCollection commands;
        private ApplicationFooter applicationFooter;

        private readonly ApplicationBuilder app = new ApplicationBuilder();

        public bool UseSpinner { get; set; }

        public ConsoleApplicationBase()
        {
            app.ApplicationServices = new ApplicationServices();
        }

        public void Initialize()
        {
            commands = CreateCommands() ?? new CommandCollection();

            ((ApplicationServices)app.ApplicationServices).Commands = commands;

            CommandCollectionItem helpCommandItem = CreateHelpCommand();

            if (helpCommandItem != null)
                commands.Add(helpCommandItem);

            applicationHeader = CreateApplicationHeader();
            applicationFooter = CreateApplicationFooter();
        }

        protected virtual ApplicationHeader CreateApplicationHeader()
        {
            return new ApplicationHeader();
        }

        protected virtual CommandCollection CreateCommands()
        {
            return new CommandCollection();
        }

        protected virtual CommandCollectionItem CreateHelpCommand()
        {
            HelpCommand helpCommand = new HelpCommand(commands);
            return new CommandCollectionItem("help", helpCommand);
        }

        protected virtual ApplicationFooter CreateApplicationFooter()
        {
            return new ApplicationFooter();
        }

        public void Run(string[] args)
        {
            try
            {
                OnStart();

                applicationHeader?.Display();

                Arguments arguments = new Arguments(args);

                if (UseSpinner)
                    Spinner.Run(() => ProcessRequest(arguments));
                else
                    ProcessRequest(arguments);
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
            finally
            {
                OnExit();
                applicationFooter?.Display();
            }
        }

        private void ProcessRequest(Arguments arguments)
        {
            RequestDelegate requestDelegate = app.Build();

            ConsoleRequestContext context = new ConsoleRequestContext(arguments);
            context.RequestServices = app.ApplicationServices;

            requestDelegate.Invoke(context);
        }

        protected virtual void OnStart()
        {
            app.Use(async (context, next) =>
            {
                await next();
            });

            app.UseUselessMiddleware();
            app.UseCommands();
        }

        protected virtual void OnExit()
        {
        }

        protected virtual void OnError(Exception ex)
        {
            CustomConsole.WriteLineError(ex);
        }
    }
}