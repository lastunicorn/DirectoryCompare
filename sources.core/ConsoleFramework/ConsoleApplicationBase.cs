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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DustInTheWind.ConsoleFramework.AppBuilder;
using DustInTheWind.ConsoleFramework.CustomMiddleware;
using DustInTheWind.ConsoleFramework.UserControls;
using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Spinners;

namespace DustInTheWind.ConsoleFramework
{
    public abstract class ConsoleApplicationBase
    {
        private ApplicationHeader applicationHeader;
        private CommandInfoCollection commands;
        private ApplicationFooter applicationFooter;

        private MiddlewareCollection middlewareCollection;

        protected IServiceProvider ServiceProvider { get; private set; }

        public bool UseSpinner { get; set; }

        public void Initialize()
        {
            IServiceCollection serviceCollection = CreateServiceCollection();
            ConfigureServices(serviceCollection);
            serviceCollection.AddSingleton(serviceCollection);
            serviceCollection.AddTransient<ICommandFactory, CommandFactory>();

            ServiceProvider = serviceCollection.BuildServiceProvider();

            commands = CreateCommands() ?? new CommandInfoCollection();
            serviceCollection.AddSingleton(commands);

            middlewareCollection = ServiceProvider.GetService<MiddlewareCollection>();

            applicationHeader = CreateApplicationHeader();
            applicationFooter = CreateApplicationFooter();
        }

        protected abstract IServiceCollection CreateServiceCollection();

        protected virtual void ConfigureServices(IServiceCollection serviceCollection)
        {
        }

        protected virtual ApplicationHeader CreateApplicationHeader()
        {
            return new ApplicationHeader();
        }

        protected virtual ApplicationFooter CreateApplicationFooter()
        {
            return new ApplicationFooter();
        }

        protected virtual CommandInfoCollection CreateCommands()
        {
            // AppDomain currentAppDomain = AppDomain.CurrentDomain;
            // Assembly[] assemblies = currentAppDomain.GetAssemblies();
            // CommandInfo[] commandInfos = assemblies
            //     .SelectMany(x => x.GetTypes())
            //     .Select(x => new CommandInfo(x))
            //     .Where(x => x.IsValidCommand)
            //     .ToArray();

            CommandInfo[] commandInfos = DiscoverCommands().ToArray();
            return new CommandInfoCollection(commandInfos);
        }

        public IEnumerable<CommandInfo> DiscoverCommands()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            string rootDirectoryPath = Path.GetDirectoryName(executingAssembly.Location);

            string[] assemblyFileNames = Directory.GetFiles(rootDirectoryPath, "*.dll");

            return assemblyFileNames
                .Select(LoadAssembly)
                .Where(x => x != null)
                .Where(x => x.FullName != executingAssembly.FullName)
                .SelectMany(GetAllTypes)
                .Select(x => new CommandInfo(x))
                .Where(x => x.IsValidCommand);
        }

        private Assembly LoadAssembly(string x)
        {
            try
            {
                return Assembly.LoadFile(x);
            }
            catch (BadImageFormatException)
            {
                string message = string.Format("Dll file is not a .NET assembly. File name = {0}", x);
                //log.WriteInfo(message);

                return null;
            }
            catch (Exception ex)
            {
                string message = string.Format("Warning: Could not load an Assembly while searching for installer plugin instances. File name = {0}", x);
                //log.WriteWarning(message, ex);

                return null;
            }
        }

        private IEnumerable<Type> GetAllTypes(Assembly x)
        {
            try
            {
                return x.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                //log.WriteWarning("Warning: Could not load a Type while searching for installer plugin instances. Loader Exceptions follows:", ex);

                // foreach (Exception exLoaderException in ex.LoaderExceptions)
                //     log.WriteWarning(exLoaderException);

                return new Type[0];
            }
            catch (Exception ex)
            {
                //log.WriteWarning("Warning: Could not load a Type while searching for UiPackage instances.", ex);
                return null;
            }
        }

        public void Run(string[] args)
        {
            try
            {
                OnStart();

                applicationHeader?.Display();

                Arguments arguments = new(args);

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
            ConsoleRequestContext context = new(arguments)
            {
                RequestServices = middlewareCollection.ApplicationServices
            };

            middlewareCollection.Execute(context);
        }

        protected virtual void OnStart()
        {
            middlewareCollection.UseExceptionHandler();
            middlewareCollection.UseUselessMiddleware();
            middlewareCollection.UseCommands();
        }

        protected virtual void OnExit()
        {
        }

        protected virtual void OnError(Exception ex)
        {
            CustomConsole.WriteLineError(ex);
        }
    }

    internal class CommandFactory : ICommandFactory
    {
        private readonly IServiceProvider serviceProvider;

        public CommandFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public ICommand Create(Type type)
        {
            return serviceProvider.GetService(type) as ICommand;
        }
    }
}