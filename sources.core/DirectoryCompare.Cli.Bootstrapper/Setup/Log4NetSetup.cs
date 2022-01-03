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

using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using log4net.Repository;

namespace DustInTheWind.DirectoryCompare.Cli.Bootstrapper.Setup
{
    internal class Log4NetSetup
    {
        public static void Setup()
        {
            Assembly assembly = Assembly.GetEntryAssembly();

            ILoggerRepository loggerRepository = LogManager.GetRepository(assembly);
            
            string assemblyFilePath = assembly.Location;
            string applicationDirectoryPath = Path.GetDirectoryName(assemblyFilePath);
            string configFilePath = Path.Combine(applicationDirectoryPath, "Log4Net.config");
            FileInfo configFileInfo = new(configFilePath);

            XmlConfigurator.Configure(loggerRepository, configFileInfo);
        }
    }
}