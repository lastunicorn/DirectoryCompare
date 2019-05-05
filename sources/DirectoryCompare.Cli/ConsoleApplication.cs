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

using DirectoryCompare.CliFramework;
using DustInTheWind.DirectoryCompare.Application;
using DustInTheWind.DirectoryCompare.Application.Disk;
using DustInTheWind.DirectoryCompare.Cli.Commands;
using MediatR;
using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Planning.Bindings.Resolvers;

namespace DustInTheWind.DirectoryCompare.Cli
{
    internal class ConsoleApplication : ConsoleApplicationBase
    {
        private readonly IMediator mediator;

        public ConsoleApplication()
        {
            mediator = BuildMediator();
        }

        private static IMediator BuildMediator()
        {
            StandardKernel kernel = new StandardKernel();
            kernel.Components.Add<IBindingResolver, ContravariantBindingResolver>();
            kernel.Bind(x => x.FromAssemblyContaining<IMediator>().SelectAllClasses().BindDefaultInterface());
            kernel.Bind(x => x.FromAssemblyContaining<ReadDiskRequest>().SelectAllClasses().InheritedFrom(typeof(IRequestHandler<,>)).BindAllInterfaces());

            kernel.Bind<IProjectLogger>().To<ProjectLogger>();

            kernel.Bind(typeof(IPipelineBehavior<,>)).To(typeof(RequestPerformanceBehaviour<,>));
            //kernel.Bind(typeof(IPipelineBehavior<,>)).To(typeof(RequestValidationBehavior<,>));

            kernel.Bind<ServiceFactory>().ToMethod(x => t => x.Kernel.TryGet(t));

            return kernel.Get<IMediator>();
        }

        protected override CommandCollection CreateCommands()
        {
            ReadDiskCommand readDiskCommand = new ReadDiskCommand(mediator);
            ReadFileCommand readFileCommand = new ReadFileCommand(mediator);
            VerifyDiskCommand verifyDiskCommand = new VerifyDiskCommand(mediator);

            return new CommandCollection
            {
                { "read-disk", readDiskCommand },
                { "read", readDiskCommand },
                { "read-file", readFileCommand },
                { "view", readFileCommand },
                { "verify-disk", verifyDiskCommand },
                { "check", verifyDiskCommand },
                { "compare-disks", new CompareDisksCommand(mediator) },
                { "compare-files", new CompareFilesCommand(mediator) },
                { "find-duplicates", new FindDuplicatesCommand(mediator) },
                { "remove-duplicates", new RemoveDuplicatesCommand(mediator) }
            };
        }
    }
}