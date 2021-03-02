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
using Ninject;

namespace DustInTheWind.DirectoryCompare.Cli
{
    internal class NinjectServiceCollection : IServiceCollection
    {
        public StandardKernel Kernel { get; }

        public NinjectServiceCollection()
        {
            Kernel = new StandardKernel();
            Kernel.Bind<IServiceProvider>().ToConstant(Kernel);
        }

        public void AddSingleton<TService>(TService implementationInstance)
            where TService : class
        {
            Kernel.Bind<TService>().ToConstant(implementationInstance).InSingletonScope();
        }

        public void AddSingleton<TService>() where TService : class
        {
            Kernel.Bind<TService>().ToSelf().InSingletonScope();
        }

        public void AddSingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            Kernel.Bind<TService>().To<TImplementation>().InSingletonScope();
        }

        public void AddTransient<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            Kernel.Bind<TService>().To<TImplementation>();
        }

        public void AddTransient(Type serviceType, Type implementationType)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            if (implementationType == null) throw new ArgumentNullException(nameof(implementationType));

            Kernel.Bind(serviceType).To(implementationType);
        }

        public void AddTransient(Type serviceType)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));

            Kernel.Bind(serviceType).ToSelf();
        }

        public IServiceProvider BuildServiceProvider()
        {
            return Kernel;
        }
    }
}