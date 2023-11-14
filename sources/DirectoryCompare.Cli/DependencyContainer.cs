// VeloCity
// Copyright (C) 2022-2023 Dust in the Wind
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
using DustInTheWind.DirectoryCompare.Cli.Application;
using DustInTheWind.DirectoryCompare.Cli.Application.PotArea.PresentPots;
using DustInTheWind.DirectoryCompare.ConfigAccess;
using DustInTheWind.DirectoryCompare.DataAccess;
using DustInTheWind.DirectoryCompare.Domain.ImportExport;
using DustInTheWind.DirectoryCompare.FileSystemAccess;
using DustInTheWind.DirectoryCompare.Infrastructure.RequestPipeline;
using DustInTheWind.DirectoryCompare.LogAccess;
using DustInTheWind.DirectoryCompare.Ports.ConfigAccess;
using DustInTheWind.DirectoryCompare.Ports.DataAccess;
using DustInTheWind.DirectoryCompare.Ports.FileSystemAccess;
using DustInTheWind.DirectoryCompare.Ports.LogAccess;
using FluentValidation;
using MediatR.Extensions.Autofac.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection.Builder;

namespace DustInTheWind.DirectoryCompare.Cli;

internal static class DependencyContainer
{
    public static void Setup(ContainerBuilder containerBuilder)
    {
        RegisterLogAccessAdapter(containerBuilder);
        RegisterConfigAccessAdapter(containerBuilder);
        RegisterDataAccessAdapter(containerBuilder);
        RegisterFileSystemAccessAdapter(containerBuilder);

        RegisterApplicationComponent(containerBuilder);
    }

    private static void RegisterLogAccessAdapter(ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterType<Log>().As<ILog>().SingleInstance();
        containerBuilder.RegisterType<ConsoleRemoveDuplicatesLog>().As<IRemoveDuplicatesLog>().SingleInstance();
    }

    private static void RegisterConfigAccessAdapter(ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterType<Config>().As<IConfig>().SingleInstance();
    }

    private static void RegisterDataAccessAdapter(ContainerBuilder containerBuilder)
    {
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
    }

    private static void RegisterFileSystemAccessAdapter(ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterType<FileSystem>().As<IFileSystem>();
    }

    private static void RegisterApplicationComponent(ContainerBuilder containerBuilder)
    {
        Assembly applicationAssembly = typeof(PresentPotsUseCase).Assembly;

        MediatRConfiguration mediatRConfiguration = MediatRConfigurationBuilder
            .Create(applicationAssembly)
            .WithAllOpenGenericHandlerTypesRegistered()
            .WithCustomPipelineBehaviors(new[] { typeof(RequestValidationBehavior<,>), typeof(RequestPerformanceBehavior<,>) })
            .Build();

        containerBuilder.RegisterMediatR(mediatRConfiguration);
        containerBuilder.RegisterAssemblyTypes(applicationAssembly)
            .Where(x => x.IsClosedTypeOf(typeof(IValidator<>)))
            .AsImplementedInterfaces();

        containerBuilder.RegisterType<RequestBus>().AsSelf();
    }
}