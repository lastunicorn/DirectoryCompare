// DirectoryCompare
// Copyright (C) 2017-2024 Dust in the Wind
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

using System.Globalization;
using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Commando.Setup.Autofac;
using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.DirectoryCompare.Cli.Presentation.PotCommands.DisplayPots;

namespace DustInTheWind.DirectoryCompare.Cli;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        try
        {
            Log4NetSetup.Setup();

            ConsoleTools.Commando.Application application = ApplicationBuilder.Create()
                .ConfigureServices(DependencyContainer.Setup)
                .RegisterCommandsFrom(typeof(DisplayPotsCommand).Assembly)
                .HandleExceptions(HandleApplicationExceptions)
                .Build();

            application.Starting += HandleApplicationStarting;
            application.Ended += HandleApplicationEnded;

            await application.RunAsync(args);
        }
        catch (Exception ex)
        {
            CustomConsole.WriteLineError(ex);
        }
    }

    private static void HandleApplicationEnded(object sender, EventArgs e)
    {
        CustomConsole.WriteLine();
    }

    private static void HandleApplicationExceptions(object sender, UnhandledApplicationExceptionEventArgs ex)
    {
        // ILog log = ServiceProvider.GetService<ILog>();
        // log.WriteError(ex.ToString());
    }

    private static void HandleApplicationStarting(object sender, EventArgs e)
    {
        CultureInfo cultureInfo = new("ro-RO");

        CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo;

        ApplicationHeader applicationHeader = new();
        applicationHeader.Display();
    }
}