// DirectoryCompare
// Copyright (C) 2017-2023 Dust in the Wind
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
using Autofac;
using Avalonia;
using Avalonia.Controls;
using DustInTheWind.Clindy.Applications;
using DustInTheWind.Clindy.Applications.PresentDuplicates;
using DustInTheWind.Clindy.ViewModels;
using DustInTheWind.Clindy.Views;
using DustInTheWind.DirectoryCompare.ConfigAccess;
using DustInTheWind.DirectoryCompare.DataAccess;
using DustInTheWind.DirectoryCompare.FileSystemAccess;
using DustInTheWind.DirectoryCompare.ImportExportAccess;
using DustInTheWind.DirectoryCompare.Infrastructure.RequestPipeline;
using DustInTheWind.DirectoryCompare.LogAccess;
using DustInTheWind.DirectoryCompare.Ports.ConfigAccess;
using DustInTheWind.DirectoryCompare.Ports.DataAccess;
using DustInTheWind.DirectoryCompare.Ports.DataAccess.ImportExport;
using DustInTheWind.DirectoryCompare.Ports.FileSystemAccess;
using DustInTheWind.DirectoryCompare.Ports.ImportExportAccess;
using DustInTheWind.DirectoryCompare.Ports.LogAccess;
using DustInTheWind.DirectoryCompare.Ports.UserAccess;
using DustInTheWind.DirectoryCompare.UserAccess;
using FluentValidation;
using MediatR.Extensions.Autofac.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection.Builder;
using ReactiveUI;
using Splat;
using Splat.Autofac;

namespace DustInTheWind.Clindy;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        // Build a new Autofac container.
        ContainerBuilder containerBuilder = new();

        containerBuilder.RegisterType<MainWindow>().AsSelf();
        containerBuilder.RegisterType<MainWindowViewModel>().AsSelf();

        RegisterAdapters(containerBuilder);
        RegisterApplication(containerBuilder);

        // Use Autofac for ReactiveUI dependency resolution.
        // After we call the method below, Locator.Current and
        // Locator.CurrentMutable start using Autofac locator.
        AutofacDependencyResolver resolver = new(containerBuilder);
        Locator.SetLocator(resolver);

        // These .InitializeX() methods will add ReactiveUI platform 
        // registrations to your container. They MUST be present if
        // you *override* the default Locator.
        Locator.CurrentMutable.InitializeSplat();
        Locator.CurrentMutable.InitializeReactiveUI();
        Locator.CurrentMutable.InitializeAvalonia();

        IContainer container = containerBuilder.Build();
        resolver.SetLifetimeScope(container);

        BuildAvaloniaApp()
            .Start(AppMain, args);
    }

    private static void AppMain(Application app, string[] args)
    {
        MainWindow? mainWindow = Locator.Current.GetService<MainWindow>();
        app.Run(mainWindow);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }

    private static void RegisterAdapters(ContainerBuilder containerBuilder)
    {
        // Log Access
        
        containerBuilder.RegisterType<Log>().As<ILog>().SingleInstance();
        containerBuilder.RegisterType<ConsoleRemoveDuplicatesLog>().As<IRemoveDuplicatesLog>().SingleInstance();
        
        // Config Access
        
        containerBuilder.RegisterType<Config>().As<IConfig>().SingleInstance();
        
        // Data Access
        
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
        
        // File System Access
        
        containerBuilder.RegisterType<FileSystem>().As<IFileSystem>();
        
        // User Access
        
        containerBuilder.RegisterType<DeletePotUi>().As<IDeletePotUi>();
        containerBuilder.RegisterType<CreateSnapshotUi>().As<ICreateSnapshotUi>();
        containerBuilder.RegisterType<DuplicateFilesUi>().As<IDuplicateFilesUi>();
        
        // System Access
        
        containerBuilder.RegisterType<DustInTheWind.DirectoryCompare.SystemAccess.SystemClock>().As<DustInTheWind.DirectoryCompare.Ports.SystemAccess.ISystemClock>();
        
        // Import/Export Access
        
        containerBuilder.RegisterType<ImportExport>().As<IImportExport>();
    }

    private static void RegisterApplication(ContainerBuilder containerBuilder)
    {
        Assembly applicationAssembly = typeof(PresentDuplicatesRequest).Assembly;

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