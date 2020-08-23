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
using System.Threading.Tasks;

namespace DustInTheWind.ConsoleFramework.AppBuilder
{
    public class MiddlewareCollection
    {
        private readonly IList<Func<RequestDelegate, RequestDelegate>> items = new List<Func<RequestDelegate, RequestDelegate>>();

        private readonly IMiddlewareFactory middlewareFactory;

        public IServiceProvider ApplicationServices { get; set; }

        public MiddlewareCollection(IMiddlewareFactory middlewareFactory)
        {
            this.middlewareFactory = middlewareFactory ?? throw new ArgumentNullException(nameof(middlewareFactory));
        }

        public MiddlewareCollection UseMiddleware<TMiddleware>()
        {
            return UseMiddleware(typeof(TMiddleware));
        }

        public MiddlewareCollection UseMiddleware(Type middlewareType)
        {
            return Use(next => CreateMiddlewareInvoker(middlewareType, next));
        }

        private RequestDelegate CreateMiddlewareInvoker(Type middlewareType, RequestDelegate next)
        {
            RequestDelegate middlewareInvoker = async context =>
            {
                IConsoleMiddleware middleware = InstantiateMiddleware(middlewareType);
                await middleware.InvokeAsync(context, next);
            };

            return middlewareInvoker;
        }

        private IConsoleMiddleware InstantiateMiddleware(Type middlewareType)
        {
            IConsoleMiddleware middleware = middlewareFactory.Create(middlewareType);

            if (middleware == null)
                throw new InvalidOperationException(string.Format("The factory {0} cannot create a middleware of type {1}.", middlewareFactory.GetType(), middlewareType));

            return middleware;
        }

        private MiddlewareCollection Use(Func<RequestDelegate, RequestDelegate> middleware)
        {
            if (middleware == null) throw new ArgumentNullException(nameof(middleware));

            items.Add(middleware);
            return this;
        }

        public void Execute(ConsoleRequestContext context)
        {
            RequestDelegate requestDelegate = Build();
            requestDelegate.Invoke(context);
        }

        private RequestDelegate Build()
        {
            RequestDelegate requestDelegate = context => Task.CompletedTask;
            
            foreach (Func<RequestDelegate, RequestDelegate> component in items.Reverse()) 
                requestDelegate = component(requestDelegate);

            return requestDelegate;
        }
    }
}