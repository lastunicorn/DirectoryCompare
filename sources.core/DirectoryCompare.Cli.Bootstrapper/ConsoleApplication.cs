// DirectoryCompare
// Copyright (C) 2017-2019 Dust in the Wind
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
using DustInTheWind.DirectoryCompare.Cli.Setup;
using DustInTheWind.DirectoryCompare.Domain.Logging;
using Ninject;

namespace DustInTheWind.DirectoryCompare.Cli
{
    internal class ConsoleApplication : ConsoleApplicationBase
    {
        private readonly KernelBase dependencyContainer;
        private readonly IProjectLogger logger;

        public ConsoleApplication()
        {
            dependencyContainer = DependencyContainerSetup.Setup();
            MediatorSetup.Setup(dependencyContainer);
            Log4NetSetup.Configure();
            logger = dependencyContainer.Get<IProjectLogger>();
        }

        protected override CommandCollection CreateCommands()
        {
            return CommandsSetup.Create(dependencyContainer);
        }

        protected override void OnStart()
        {
            logger.Open();

            base.OnStart();
        }

        protected override void OnExit()
        {
            logger.Close();

            base.OnExit();
        }

        protected override void OnError(Exception ex)
        {
            logger.Error(ex.ToString());
        }
    }
}