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
using DustInTheWind.ConsoleFramework;
using DustInTheWind.ConsoleFramework.AppBuilder;
using DustInTheWind.ConsoleFramework.Logging;
using DustInTheWind.DirectoryCompare.Cli.Bootstrapper.Setup;
using DustInTheWind.DirectoryCompare.Cli.Presentation;
using DustInTheWind.DirectoryCompare.DataAccess;
using DustInTheWind.DirectoryCompare.Domain.DataAccess;
using DustInTheWind.DirectoryCompare.Domain.ImportExport;

namespace DustInTheWind.DirectoryCompare.Cli.Bootstrapper
{
    internal class ConsoleApplication : ConsoleApplicationBase
    {
        public ConsoleApplication()
        {
            Log4NetSetup.Setup();

            // This is a trick to force loading the presentation assembly.
            DummyClass dummyClass = new DummyClass();
        }

        protected override IServiceCollection CreateServiceCollection()
        {
            return new NinjectServiceCollection();
        }

        protected override void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ILog, Log>();
            serviceCollection.AddSingleton<Domain.Logging.ILog, Logging.Log>();
            serviceCollection.AddTransient<IPotRepository, PotRepository>();
            serviceCollection.AddTransient<IBlackListRepository, BlackListRepository>();
            serviceCollection.AddTransient<ISnapshotRepository, SnapshotRepository>();
            serviceCollection.AddTransient<IPotImportExport, PotImportExport>();

            MediatorSetup.Setup(serviceCollection);

            serviceCollection.AddTransient<IMiddlewareFactory, MiddlewareFactory>();

            base.ConfigureServices(serviceCollection);
        }

        protected override void OnError(Exception ex)
        {
            ILog log = ServiceProvider.GetService<ILog>();
            log.WriteError(ex.ToString());
        }
    }
}