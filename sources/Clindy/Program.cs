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
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using Autofac;
using Avalonia;
using Avalonia.Controls;
using DustInTheWind.Clindy.Presentation.Views;
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
        CultureInfo.CurrentCulture = new CultureInfo("ro-RO");
        
        // Build a new Autofac container.
        ContainerBuilder containerBuilder = new();

        Setup.RegisterDependencies(containerBuilder);

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
}