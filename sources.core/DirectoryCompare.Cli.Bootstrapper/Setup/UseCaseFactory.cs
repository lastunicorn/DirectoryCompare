using System;
using DustInTheWind.RequestR;

namespace DustInTheWind.DirectoryCompare.Cli.Setup
{
    internal class UseCaseFactory : UseCaseFactoryBase
    {
        private readonly IServiceProvider serviceProvider;

        public UseCaseFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        protected override object CreateInternal(Type type)
        {
            return serviceProvider.GetService(type);
        }
    }
}