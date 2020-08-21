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

namespace DustInTheWind.ConsoleFramework.AppBuilder
{
    public static class UseMiddlewareExtensions
    {
        public static IApplicationBuilder UseMiddleware<TMiddleware>(this IApplicationBuilder app)
        {
            return app.UseMiddleware(typeof(TMiddleware));
        }

        ///// <summary>
        ///// Adds a middleware type to the application's request pipeline.
        ///// </summary>
        ///// <param name="app">The <see cref="IApplicationBuilder"/> instance.</param>
        ///// <param name="middleware">The middleware type.</param>
        ///// <param name="args">The arguments to pass to the middleware type instance's constructor.</param>
        ///// <returns>The <see cref="IApplicationBuilder"/> instance.</returns>
        //public static IApplicationBuilder UseMiddleware(this IApplicationBuilder app, Type middleware, params object[] args)
        //{
        //    if (typeof(IMiddleware).GetTypeInfo().IsAssignableFrom(middleware.GetTypeInfo()))
        //    {
        //        // IMiddleware doesn't support passing args directly since it's
        //        // activated from the container
        //        if (args.Length > 0)
        //        {
        //            throw new NotSupportedException(string.Format("IMiddleware {0} doesn't support passing args directly.", typeof(IMiddleware)));
        //        }

        //        return UseMiddleware(app, middleware);
        //    }

        //    IServiceProvider applicationServices = app.ApplicationServices;

        //    return app.Use(next =>
        //    {
        //        MethodInfo[] invokeMethods = middleware.GetMethods(BindingFlags.Instance | BindingFlags.Public)
        //            .Where(m =>  string.Equals(m.Name, InvokeMethodName, StringComparison.Ordinal)
        //                        || string.Equals(m.Name, InvokeAsyncMethodName, StringComparison.Ordinal))
        //            .ToArray();

        //        if (invokeMethods.Length > 1)
        //            throw new InvalidOperationException(Resources.FormatException_UseMiddleMutlipleInvokes(InvokeMethodName, InvokeAsyncMethodName));

        //        if (invokeMethods.Length == 0)
        //            throw new InvalidOperationException(Resources.FormatException_UseMiddlewareNoInvokeMethod(InvokeMethodName, InvokeAsyncMethodName, middleware));

        //        MethodInfo methodInfo = invokeMethods[0];
                
        //        if (!typeof(Task).IsAssignableFrom(methodInfo.ReturnType))
        //            throw new InvalidOperationException(Resources.FormatException_UseMiddlewareNonTaskReturnType(InvokeMethodName, InvokeAsyncMethodName, nameof(Task)));

        //        ParameterInfo[] parameters = methodInfo.GetParameters();
        //        if (parameters.Length == 0 || parameters[0].ParameterType != typeof(HttpContext))
        //            throw new InvalidOperationException(Resources.FormatException_UseMiddlewareNoParameters(InvokeMethodName, InvokeAsyncMethodName, nameof(HttpContext)));

        //        object[] ctorArgs = new object[args.Length + 1];
        //        ctorArgs[0] = next;
        //        Array.Copy(args, 0, ctorArgs, 1, args.Length);
        //        var instance = ActivatorUtilities.CreateInstance(app.ApplicationServices, middleware, ctorArgs);
        //        if (parameters.Length == 1)
        //        {
        //            return (RequestDelegate)methodInfo.CreateDelegate(typeof(RequestDelegate), instance);
        //        }

        //        var factory = Compile<object>(methodInfo, parameters);

        //        return context =>
        //        {
        //            var serviceProvider = context.RequestServices ?? applicationServices;
        //            if (serviceProvider == null)
        //            {
        //                throw new InvalidOperationException(Resources.FormatException_UseMiddlewareIServiceProviderNotAvailable(nameof(IServiceProvider)));
        //            }

        //            return factory(instance, context, serviceProvider);
        //        };
        //    });
        //}

        public static IApplicationBuilder UseMiddleware(this IApplicationBuilder app, Type middlewareType)
        {
            return app.Use(next =>
            {
                return async context =>
                {
                    IMiddlewareFactory middlewareFactory = (IMiddlewareFactory)context.RequestServices.GetService(typeof(IMiddlewareFactory));

                    if (middlewareFactory == null)
                        throw new InvalidOperationException(string.Format("There is not instance of the {0} configured.", typeof(IMiddlewareFactory)));

                    IMiddleware middleware = middlewareFactory.Create(middlewareType);

                    if (middleware == null)
                        throw new InvalidOperationException(string.Format("The factory {0} cannot create a middleware of type {1}.", middlewareFactory.GetType(), middlewareType));

                    try
                    {
                        await middleware.InvokeAsync(context, next);
                    }
                    finally
                    {
                        middlewareFactory.Release(middleware);
                    }
                };
            });
        }
    }
}