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
using DustInTheWind.ConsoleFramework;
using DustInTheWind.ConsoleFramework.Logging;

namespace DustInTheWind.DirectoryCompare.Cli
{
    public class CommandProvider
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILog log;

        public CommandProvider(IServiceProvider serviceProvider, ILog log)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public IEnumerable<ICommand> ProvideAll()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            string rootDirectoryPath = Path.GetDirectoryName(executingAssembly.Location);

            string[] assemblyFileNames = Directory.GetFiles(rootDirectoryPath, "*.dll");

            return assemblyFileNames
                .Select(LoadAssembly)
                .Where(x => x != null)
                .Where(x => x.FullName != executingAssembly.FullName)
                .SelectMany(GetAllTypes)
                .Where(x => x != null && x.IsClass && !x.IsAbstract && typeof(ICommand).IsAssignableFrom(x))
                .Select(x => serviceProvider.GetService(x))
                .Cast<ICommand>();
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
                log.WriteInfo(message);

                return null;
            }
            catch (Exception ex)
            {
                string message = string.Format("Warning: Could not load an Assembly while searching for installer plugin instances. File name = {0}", x);
                log.WriteWarning(message, ex);

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
                log.WriteWarning("Warning: Could not load a Type while searching for installer plugin instances. Loader Exceptions follows:", ex);

                foreach (Exception exLoaderException in ex.LoaderExceptions)
                    log.WriteWarning(exLoaderException);

                return new Type[0];
            }
            catch (Exception ex)
            {
                log.WriteWarning("Warning: Could not load a Type while searching for UiPackage instances.", ex);
                return null;
            }
        }
    }
}