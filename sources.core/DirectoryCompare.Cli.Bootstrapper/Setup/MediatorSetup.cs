// // DirectoryCompare
// // Copyright (C) 2017-2020 Dust in the Wind
// // 
// // This program is free software: you can redistribute it and/or modify
// // it under the terms of the GNU General Public License as published by
// // the Free Software Foundation, either version 3 of the License, or
// // (at your option) any later version.
// // 
// // This program is distributed in the hope that it will be useful,
// // but WITHOUT ANY WARRANTY; without even the implied warranty of
// // MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// // GNU General Public License for more details.
// // 
// // You should have received a copy of the GNU General Public License
// // along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// using DustInTheWind.DirectoryCompare.Application.SnapshotArea.CreateSnapshot;
// using DustInTheWind.DirectoryCompare.Infrastructure.RequestHandlingPipeline;
// using FluentValidation;
// using MediatR;
// using Ninject;
// using Ninject.Planning.Bindings.Resolvers;
//
// namespace DustInTheWind.DirectoryCompare.Cli.Bootstrapper.Setup
// {
//     internal static class MediatorSetup
//     {
//         public static void Setup(IKernel kernel)
//         {
//             kernel.Components.Add<IBindingResolver, ContravariantBindingResolver>();
//
//             kernel.Bind(x => x.FromAssemblyContaining<IMediator>().SelectAllClasses().BindDefaultInterface());
//             kernel.Bind(x => x.FromAssemblyContaining<CreateSnapshotRequest>().SelectAllClasses().InheritedFrom(typeof(IRequestHandler<,>)).BindAllInterfaces());
//             kernel.Bind(x => x.FromAssemblyContaining<CreateSnapshotRequestValidator>().SelectAllClasses().InheritedFrom(typeof(AbstractValidator<>)).BindDefaultInterfaces());
//
//             kernel
//                 .Bind(typeof(IPipelineBehavior<,>))
//                 .To(typeof(RequestPerformanceBehavior<,>));
//
//             kernel
//                 .Bind(typeof(IPipelineBehavior<,>))
//                 .To(typeof(RequestValidationBehavior<,>));
//
//             kernel
//                 .Bind<ServiceFactory>()
//                 .ToMethod(x =>
//                 {
//                     return type => x.Kernel.TryGet(type);
//                 });
//         }
//     }
// }