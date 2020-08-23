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
using DustInTheWind.DirectoryCompare.Application.CreateSnapshot;
using DustInTheWind.DirectoryCompare.Infrastructure.Performance;
using DustInTheWind.DirectoryCompare.Infrastructure.Validation;
using FluentValidation;
using MediatR;
using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Planning.Bindings.Resolvers;

namespace DustInTheWind.DirectoryCompare.Cli.Setup
{
    internal static class MediatorSetup
    {
        public static void Setup(IServiceCollection serviceCollection)
        {
            KernelBase dependencyContainer = ((NinjectServiceCollection)serviceCollection).Kernel;

            dependencyContainer.Components.Add<IBindingResolver, ContravariantBindingResolver>();

            dependencyContainer.Bind(x => x.FromAssemblyContaining<IMediator>().SelectAllClasses().BindDefaultInterface());
            dependencyContainer.Bind(x => x.FromAssemblyContaining<CreateSnapshotRequest>().SelectAllClasses().InheritedFrom(typeof(IRequestHandler<,>)).BindAllInterfaces());
            dependencyContainer.Bind(x => x.FromAssemblyContaining<CreateSnapshotRequestValidator>().SelectAllClasses().InheritedFrom(typeof(AbstractValidator<>)).BindDefaultInterfaces());

            dependencyContainer
                .Bind(typeof(IPipelineBehavior<,>))
                .To(typeof(RequestPerformanceBehavior<,>));
            dependencyContainer
                .Bind(typeof(IPipelineBehavior<,>))
                .To(typeof(RequestValidationBehavior<,>));

            dependencyContainer
                .Bind<ServiceFactory>()
                .ToMethod(x =>
                {
                    return t =>
                    {
                        object a = dependencyContainer.TryGet(t);
                        return x.Kernel.TryGet(t);
                    };
                });
        }
    }
}