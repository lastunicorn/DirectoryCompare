using System;

namespace DustInTheWind.ConsoleFramework
{
    public interface IServiceCollection
    {
        void AddSingleton<TService>(TService implementationInstance)
            where TService : class;
        
        void AddSingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;
        
        void AddTransient<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;

        void AddTransient(Type serviceType, Type implementationType);

        IServiceProvider BuildServiceProvider();
    }
}