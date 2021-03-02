using System;
using DustInTheWind.RequestR;

namespace DustInTheWind.DirectoryCompare.Cli.Setup
{
    internal class RequestHandlerFactory : IRequestHandlerFactory
    {
        private readonly IServiceProvider serviceProvider;

        public RequestHandlerFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public T Create<T>()
        {
            return (T)serviceProvider.GetService(typeof(T));
        }

        public object Create(Type type)
        {
            return serviceProvider.GetService(type);
        }
    }
}