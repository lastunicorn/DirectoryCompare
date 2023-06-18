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

using System.Reflection;
using Autofac;
using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.ConsoleTools.Commando.Setup.Autofac;
using DustInTheWind.DirectoryCompare.Application;
using DustInTheWind.DirectoryCompare.Application.PotArea.PresentPots;
using DustInTheWind.DirectoryCompare.Cli.Bootstrapper.Setup;
using DustInTheWind.DirectoryCompare.Cli.Presentation.PotCommands;
using DustInTheWind.DirectoryCompare.DataAccess;
using DustInTheWind.DirectoryCompare.Domain.ImportExport;
using DustInTheWind.DirectoryCompare.Domain.PotModel;
using DustInTheWind.DirectoryCompare.Infrastructure;
using DustInTheWind.DirectoryCompare.LogAccess;
using DustInTheWind.DirectoryCompare.Ports.DataAccess;
using DustInTheWind.DirectoryCompare.Ports.LogAccess;
using MediatR.Extensions.Autofac.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection.Builder;

namespace DustInTheWind.DirectoryCompare.Cli.Bootstrapper
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            try
            {
                Log4NetSetup.Setup();
                
                ConsoleTools.Commando.Application application = ApplicationBuilder.Create()
                    .ConfigureServices(containerBuilder =>
                    {
                        containerBuilder.RegisterType<ConsoleRemoveDuplicatesLog>().As<IRemoveDuplicatesLog>().SingleInstance();
                        containerBuilder.RegisterType<Log>().As<ILog>().SingleInstance();
                        
                        containerBuilder.RegisterType<PotRepository>().As<IPotRepository>();
                        containerBuilder.RegisterType<BlackListRepository>().As<IBlackListRepository>();
                        containerBuilder.RegisterType<SnapshotRepository>().As<ISnapshotRepository>();
                        containerBuilder.RegisterType<PotImportExport>().As<IPotImportExport>();
                        
                        Assembly applicationAssembly = typeof(PresentPotsUseCase).Assembly;
                        
                        MediatRConfiguration mediatRConfiguration = MediatRConfigurationBuilder
                            .Create(applicationAssembly)
                            .WithAllOpenGenericHandlerTypesRegistered()
                            .Build();
                        
                        containerBuilder.RegisterMediatR(mediatRConfiguration);
                        
                        containerBuilder.RegisterType<RequestBus>().AsSelf();
                        containerBuilder.RegisterType<SnapshotFactory>().AsSelf();
                    })
                    .RegisterCommandsFrom(typeof(DisplayPotsCommand).Assembly)
                    .HandleExceptions(EventHandler)
                    .Build();

                await application.RunAsync(args);

            }
            catch (Exception ex)
            {
                CustomConsole.WriteLineError(ex);
            }
        }

        private static void EventHandler(object sender, UnhandledApplicationExceptionEventArgs ex)
        {
            // ILog log = ServiceProvider.GetService<ILog>();
            // log.WriteError(ex.ToString());
        }
    }
}