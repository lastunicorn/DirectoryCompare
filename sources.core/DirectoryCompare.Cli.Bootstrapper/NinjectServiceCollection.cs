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

        public IServiceProvider BuildServiceProvider()
        {
            return Kernel;
        }
    }
}