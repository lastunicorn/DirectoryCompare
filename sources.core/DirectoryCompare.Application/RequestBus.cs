// DirectoryCompare
// Copyright (C) 2017-2019 Dust in the Wind
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
using System.Threading.Tasks;

namespace DustInTheWind.DirectoryCompare.Application
{
    public interface IRequestBus
    {
        TResponse ProcessRequest<TRequest, TResponse>(TRequest request);
        Task<TResponse> ProcessRequestAsync<TRequest, TResponse>(TRequest request);
    }

    public class RequestBus : IRequestBus
    {
        private readonly IRequestHandlerFactory requestHandlerFactory;
        private readonly Dictionary<Type, Type> handlers = new Dictionary<Type, Type>();

        public RequestBus()
        {
            requestHandlerFactory = new RequestHandlerFactory();
        }

        public RequestBus(IRequestHandlerFactory requestHandlerFactory)
        {
            this.requestHandlerFactory = requestHandlerFactory ?? throw new ArgumentNullException(nameof(requestHandlerFactory));
        }

        public void Register(Type requestType, Type requestHandlerType)
        {
            if (handlers.ContainsKey(requestType))
                throw new Exception("The type " + requestType.FullName + " is already registered.");

            handlers.Add(requestType, requestHandlerType);
        }

        public TResponse ProcessRequest<TRequest, TResponse>(TRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            Type requestType = typeof(TRequest);

            if (!handlers.ContainsKey(requestType))
                throw new Exception("No handler is registered for the specified request.");

            Type requestHandlerType = handlers[requestType];
            IRequestHandler<TRequest, TResponse> requestHandler = (IRequestHandler<TRequest, TResponse>)requestHandlerFactory.Create(requestHandlerType);

            return requestHandler.Handle(request);
        }

        public Task<TResponse> ProcessRequestAsync<TRequest, TResponse>(TRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            Type requestType = typeof(TRequest);

            if (!handlers.ContainsKey(requestType))
                throw new Exception("No handler is registered for the specified request.");

            Type requestHandlerType = handlers[requestType];
            IRequestHandler<TRequest, TResponse> requestHandler = (IRequestHandler<TRequest, TResponse>)requestHandlerFactory.Create(requestHandlerType);

            return Task.Run(() => requestHandler.Handle(request));
        }
    }

    public interface IRequestHandler<in TRequest, out TResponse>
    {
        TResponse Handle(TRequest request);
    }

    public interface IRequestHandlerFactory
    {
        T Create<T>();
        object Create(Type type);
    }

    public class RequestHandlerFactory : IRequestHandlerFactory
    {
        public T Create<T>()
        {
            return Activator.CreateInstance<T>();
        }

        public object Create(Type type)
        {
            return Activator.CreateInstance(type);
        }
    }
}