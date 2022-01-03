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
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
        private CommandPool commands;
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
            serviceCollection.AddTransient<IViewFactory, ViewFactory>();

            ServiceProvider = serviceCollection.BuildServiceProvider();

            commands = CreateCommands() ?? new CommandPool();
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

        private CommandPool CreateCommands()
        {
            CommandInfo[] commandInfos = DiscoverCommands().ToArray();
            return new CommandPool(commandInfos);
        }

        private IEnumerable<CommandInfo> DiscoverCommands()
        {
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            AssemblyName[] allAssemblyNames = entryAssembly.GetReferencedAssemblies();

            Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            IEnumerable<AssemblyName> assemblyNamesToBeLoaded = allAssemblyNames
                .Where(x => loadedAssemblies.Count(z => z.GetName() == x) == 0)
                .ToList();

            foreach (AssemblyName assemblyName in assemblyNamesToBeLoaded)
                AppDomain.CurrentDomain.Load(assemblyName);

            Assembly[] allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            IEnumerable<Type> allTypes = allAssemblies
                .SelectMany(GetAllTypes);

            List<Type> commandTypes = new();
            List<Type> viewTypes = new();

            foreach (Type type in allTypes)
            {
                if (CommandInfo.IsCommand(type))
                    commandTypes.Add(type);
                else if (CommandInfo.IsView(type))
                    viewTypes.Add(type);
            }

            return commandTypes
                .Select(x =>
                {
                    Type viewType = viewTypes
                        .FirstOrDefault(z => IsViewForCommand(z, x));

                    return new CommandInfo(x, viewType);
                })
                .Where(x => x.IsValidCommand);
        }

        private static bool IsViewForCommand(Type viewType, Type commandType)
        {
            return viewType.GetInterfaces()
                .Any(x =>
                {
                    if (!x.IsGenericType)
                        return false;

                    if (x.GetGenericTypeDefinition() != typeof(IView<>))
                        return false;

                    Type[] genericArguments = x.GetGenericArguments();

                    if (genericArguments.Length != 1)
                        return false;

                    return genericArguments[0] == commandType;
                });
        }

        private IEnumerable<Type> GetAllTypes(Assembly x)
        {
            try
            {
                return x.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                //log.WriteWarning("Warning: Could not load a Type while searching for command instances. Loader Exceptions follows:", ex);

                // foreach (Exception exLoaderException in ex.LoaderExceptions)
                //     log.WriteWarning(exLoaderException);

                return new Type[0];
            }
            catch (Exception ex)
            {
                //log.WriteWarning("Warning: Could not load a Type while searching for command instances.", ex);
                return null;
            }
        }

        public async Task Run(string[] args)
        {
            try
            {
                OnStart();

                applicationHeader?.Display();

                Arguments arguments = new(args);

                if (UseSpinner)
                    await Spinner.Run(async () => await ProcessRequest(arguments));
                else
                    await ProcessRequest(arguments);
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

        private async Task ProcessRequest(Arguments arguments)
        {
            ConsoleRequestContext context = new(arguments)
            {
                RequestServices = middlewareCollection.ApplicationServices
            };

            await middlewareCollection.Execute(context);
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
}