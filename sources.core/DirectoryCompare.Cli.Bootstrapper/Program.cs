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

using System.Globalization;
using System.Reflection;
using Autofac;
using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Commando.Setup.Autofac;
using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.DirectoryCompare.Cli.Application;
using DustInTheWind.DirectoryCompare.Cli.Application.PotArea.PresentPots;
using DustInTheWind.DirectoryCompare.Cli.Presentation.PotCommands;
using DustInTheWind.DirectoryCompare.ConfigAccess;
using DustInTheWind.DirectoryCompare.DataAccess;
using DustInTheWind.DirectoryCompare.Domain.ImportExport;
using DustInTheWind.DirectoryCompare.FileSystemAccess;
using DustInTheWind.DirectoryCompare.Infrastructure;
using DustInTheWind.DirectoryCompare.LogAccess;
using DustInTheWind.DirectoryCompare.Ports.ConfigAccess;
using DustInTheWind.DirectoryCompare.Ports.DataAccess;
using DustInTheWind.DirectoryCompare.Ports.FileSystemAccess;
using DustInTheWind.DirectoryCompare.Ports.LogAccess;
using MediatR.Extensions.Autofac.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection.Builder;

namespace DustInTheWind.DirectoryCompare.Cli.Bootstrapper;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        try
        {
            ApplicationHeader applicationHeader = new();
            applicationHeader.Display();

            Log4NetSetup.Setup();

            ConsoleTools.Commando.Application application = ApplicationBuilder.Create()
                .ConfigureServices(ConfigureServices)
                .RegisterCommandsFrom(typeof(DisplayPotsCommand).Assembly)
                .HandleExceptions(EventHandler)
                .Build();

            application.Starting += HandleStarting;
            
            await application.RunAsync(args);
        }
        catch (Exception ex)
        {
            CustomConsole.WriteLineError(ex);
        }
    }

    private static void ConfigureServices(ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterType<ConsoleRemoveDuplicatesLog>().As<IRemoveDuplicatesLog>().SingleInstance();
        containerBuilder.RegisterType<Log>().As<ILog>().SingleInstance();
        containerBuilder.RegisterType<Config>().As<IConfig>().SingleInstance();

        containerBuilder
            .Register(x =>
            {
                IConfig config = x.Resolve<IConfig>();
                
                Database database = new();
                database.Open(config.ConnectionString);
                
                return database;
            })
            .AsSelf()
            .SingleInstance();
        containerBuilder.RegisterType<PotRepository>().As<IPotRepository>();
        containerBuilder.RegisterType<BlackListRepository>().As<IBlackListRepository>();
        containerBuilder.RegisterType<SnapshotRepository>().As<ISnapshotRepository>();
        containerBuilder.RegisterType<PotImportExport>().As<IPotImportExport>();
        containerBuilder.RegisterType<FileSystem>().As<IFileSystem>();

        Assembly applicationAssembly = typeof(PresentPotsUseCase).Assembly;

        MediatRConfiguration mediatRConfiguration = MediatRConfigurationBuilder
            .Create(applicationAssembly)
            .WithAllOpenGenericHandlerTypesRegistered()
            .Build();

        containerBuilder.RegisterMediatR(mediatRConfiguration);

        containerBuilder.RegisterType<RequestBus>().AsSelf();
        containerBuilder.RegisterType<SnapshotFactory>().AsSelf();
    }

    private static void EventHandler(object sender, UnhandledApplicationExceptionEventArgs ex)
    {
        // ILog log = ServiceProvider.GetService<ILog>();
        // log.WriteError(ex.ToString());
    }

    private static void HandleStarting(object sender, EventArgs e)
    {
        CultureInfo cultureInfo = new("ro-RO");
        
        CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo;
    }
}