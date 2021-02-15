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
using DustInTheWind.ConsoleFramework;
using DustInTheWind.ConsoleFramework.AppBuilder;
using DustInTheWind.ConsoleFramework.Logging;
using DustInTheWind.DirectoryCompare.Cli.Setup;
using DustInTheWind.DirectoryCompare.Cli.UI.Commands;
using DustInTheWind.DirectoryCompare.DataAccess;
using DustInTheWind.DirectoryCompare.Domain.DataAccess;
using DustInTheWind.DirectoryCompare.Domain.ImportExport;
using DustInTheWind.DirectoryCompare.Infrastructure.Logging;

namespace DustInTheWind.DirectoryCompare.Cli
{
    internal class ConsoleApplication : ConsoleApplicationBase
    {
        public ConsoleApplication()
        {
            Log4NetSetup.Configure();
        }

        protected override IServiceCollection CreateServiceCollection()
        {
            return new NinjectServiceCollection();
        }

        protected override void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IProjectLogger, Log4NetLogger>();
            serviceCollection.AddTransient<IPotRepository, PotRepository>();
            serviceCollection.AddTransient<IBlackListRepository, BlackListRepository>();
            serviceCollection.AddTransient<ISnapshotRepository, SnapshotRepository>();
            serviceCollection.AddTransient<IPotImportExport, PotImportExport>();

            MediatorSetup.Setup(serviceCollection);

            serviceCollection.AddTransient<IMiddlewareFactory, MiddlewareFactory>();

            base.ConfigureServices(serviceCollection);
        }

        protected override CommandCollection CreateCommands()
        {
            CommandProvider commandProvider = ServiceProvider.GetService<CommandProvider>();

            List<ICommand> commands = commandProvider.ProvideAll().ToList();
            return new CommandCollection(commands);
        }

        protected override void OnError(Exception ex)
        {
            IProjectLogger logger = ServiceProvider.GetService<IProjectLogger>();
            logger.WriteError(ex.ToString());
        }
    }
}