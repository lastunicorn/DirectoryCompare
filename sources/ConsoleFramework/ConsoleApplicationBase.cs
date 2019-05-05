// DirectoryCompare
// Copyright (C) 2017-2019 Dust in the Wind
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

using DirectoryCompare.CliFramework.Commands;
using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Spinners;
using System;
using DirectoryCompare.CliFramework.UserControls;

namespace DirectoryCompare.CliFramework
{
    public class ConsoleApplicationBase
    {
        private ApplicationHeader applicationHeader;
        private CommandCollection commands;
        private ApplicationFooter applicationFooter;

        public bool UseSpinner { get; set; }

        public void Initialize()
        {
            commands = CreateCommands() ?? new CommandCollection();

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

                ICommand command = commands.SelectCommand(arguments);

                if (UseSpinner)
                    Spinner.Run(command.Execute);
                else
                    command.Execute();
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

        protected virtual void OnStart()
        {
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