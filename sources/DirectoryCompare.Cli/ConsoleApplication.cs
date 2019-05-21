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
using DirectoryCompare.CliFramework;
using DustInTheWind.DirectoryCompare.Application;
using DustInTheWind.DirectoryCompare.Application.Snapshots;
using DustInTheWind.DirectoryCompare.Cli.Commands;
using DustInTheWind.DirectoryCompare.DiskAnalysis;
using FluentValidation;
using MediatR;
using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Planning.Bindings.Resolvers;

namespace DustInTheWind.DirectoryCompare.Cli
{
    internal class ConsoleApplication : ConsoleApplicationBase
    {
        private readonly StandardKernel dependencyContainer;
        private readonly IMediator mediator;

        public ConsoleApplication()
        {
            dependencyContainer = BuildDependencyContainer();
            ConfigureDependencyContainer();
            mediator = BuildMediator();
        }

        private static StandardKernel BuildDependencyContainer()
        {
            return new StandardKernel();
        }

        private IMediator BuildMediator()
        {
            dependencyContainer.Components.Add<IBindingResolver, ContravariantBindingResolver>();
            dependencyContainer.Bind(x => x.FromAssemblyContaining<IMediator>().SelectAllClasses().BindDefaultInterface());
            dependencyContainer.Bind(x => x.FromAssemblyContaining<CreateSnapshotRequest>().SelectAllClasses().InheritedFrom(typeof(IRequestHandler<,>)).BindAllInterfaces());
            dependencyContainer.Bind(x => x.FromAssemblyContaining<CreateSnapshotRequestValidator>().SelectAllClasses().InheritedFrom(typeof(AbstractValidator<>)).BindDefaultInterfaces());

            dependencyContainer.Bind(typeof(IPipelineBehavior<,>)).To(typeof(RequestPerformanceBehaviour<,>));
            dependencyContainer.Bind(typeof(IPipelineBehavior<,>)).To(typeof(RequestValidationBehavior<,>));

            dependencyContainer.Bind<ServiceFactory>().ToMethod(x => t => x.Kernel.TryGet(t));

            dependencyContainer.Bind<IDiskAnalyzerFactory>().To<DiskAnalyzerFactory>().InSingletonScope();

            return dependencyContainer.Get<IMediator>();
        }

        private void ConfigureDependencyContainer()
        {
            dependencyContainer.Bind<IProjectLogger>().To<ProjectLogger>().InSingletonScope();
        }

        protected override CommandCollection CreateCommands()
        {
            CreateSnapshotCommand createSnapshotCommand = new CreateSnapshotCommand(mediator);
            ViewSnapshotCommand viewSnapshotCommand = new ViewSnapshotCommand(mediator);
            VerifyDiskCommand verifyDiskCommand = new VerifyDiskCommand(mediator);

            return new CommandCollection
            {
                { "snapshot", createSnapshotCommand },
                { "view-snapshot", viewSnapshotCommand },
                { "verify-path", verifyDiskCommand },
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