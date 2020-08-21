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
using System.Linq;
using System.Reflection;
using Ninject.Components;
using Ninject.Infrastructure;
using Ninject.Planning.Bindings;
using Ninject.Planning.Bindings.Resolvers;

namespace DustInTheWind.DirectoryCompare.Cli
{
    public class ContravariantBindingResolver : NinjectComponent, IBindingResolver
    {
        /// <summary>
        /// Returns any bindings from the specified collection that match the specified service.
        /// </summary>
        public IEnumerable<IBinding> Resolve(Multimap<Type, IBinding> bindings, Type service)
        {
            if (service.IsGenericType)
            {
                Type genericType = service.GetGenericTypeDefinition();
                Type[] genericArguments = genericType.GetGenericArguments();

                if (genericArguments.Length == 1 && genericArguments.Single().GenericParameterAttributes.HasFlag(GenericParameterAttributes.Contravariant))
                {
                    Type argument = service.GetGenericArguments().Single();

                    return bindings
                        .Where(x =>
                            x.Key.IsGenericType &&
                            x.Key.GetGenericTypeDefinition() == genericType &&
                            x.Key.GetGenericArguments().Single() != argument &&
                            x.Key.GetGenericArguments().Single().IsAssignableFrom(argument))
                        .SelectMany(x => x.Value);
                }
            }

            return Enumerable.Empty<IBinding>();
        }
    }
}