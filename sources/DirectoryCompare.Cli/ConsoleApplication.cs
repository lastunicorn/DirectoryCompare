﻿// DirectoryCompare
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
using DirectoryCompare.CliFramework;
using DustInTheWind.DirectoryCompare.Cli.Commands;
using DustInTheWind.DirectoryCompare.Cli.Setup;
using DustInTheWind.DirectoryCompare.Logging;
using MediatR;
using Ninject;

namespace DustInTheWind.DirectoryCompare.Cli
{
    internal class ConsoleApplication : ConsoleApplicationBase
    {
        private readonly KernelBase dependencyContainer;
        private readonly IMediator mediator;

        public ConsoleApplication()
        {
            dependencyContainer = DependencyContainerSetup.Setup();
            mediator = MediatorSetup.Setup(dependencyContainer);
        }

        protected override CommandCollection CreateCommands()
        {
            return new CommandCollection
            {
                { "init", new CreateProjectCommand(mediator) },
                { "snapshot", new CreateSnapshotCommand(mediator) },
                { "view-snapshot", new ViewSnapshotCommand(mediator) },
                { "verify-path", new VerifyDiskCommand(mediator) },
                { "compare-paths", new ComparePathsCommand(mediator) },
                { "compare-snapshots", new CompareSnapshotsCommand(mediator) },
                { "find-duplicates", new FindDuplicatesCommand(mediator) },
                { "remove-duplicates", new RemoveDuplicatesCommand(mediator) }
            };
        }

        protected override void OnStart()
        {
            IProjectLogger logger = dependencyContainer.Get<IProjectLogger>();
            logger.Open();

            base.OnStart();
        }

        protected override void OnExit()
        {
            IProjectLogger logger = dependencyContainer.Get<IProjectLogger>();
            logger.Close();

            base.OnExit();
        }

        protected override void OnError(Exception ex)
        {
            IProjectLogger logger = dependencyContainer.Get<IProjectLogger>();
            logger.Error(ex.ToString());
        }
    }
}