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
using System.Collections.Generic;
using System.Reflection;
using DustInTheWind.ConsoleFramework;
using DustInTheWind.RequestR;

namespace DustInTheWind.DirectoryCompare.Cli.Setup
{
    internal static class MediatorSetup
    {
        public static void Setup(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<UseCaseFactoryBase, UseCaseFactory>();
            serviceCollection.AddSingleton<RequestBus>();
            serviceCollection.AddAllHandlersAndValidators();
        }

        private static void AddAllHandlersAndValidators(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));

            AppDomain appDomain = AppDomain.CurrentDomain;

            Assembly[] assemblies = appDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                IEnumerable<Type> useCasesOrValidators = assembly.GetAllUseCasesAndRequestValidators();

                foreach (Type handlerOrValidatorType in useCasesOrValidators)
                    serviceCollection.AddTransient(handlerOrValidatorType);
            }
        }
    }
}