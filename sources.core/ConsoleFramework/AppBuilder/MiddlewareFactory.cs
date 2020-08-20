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
using DustInTheWind.ConsoleFramework.CustomMiddleware;

namespace DustInTheWind.ConsoleFramework.AppBuilder
{
    public class MiddlewareFactory : IMiddlewareFactory
    {
        private readonly CommandCollection commands;

        public MiddlewareFactory(CommandCollection commands)
        {
            this.commands = commands ?? throw new ArgumentNullException(nameof(commands));
        }

        public IMiddleware Create(Type middlewareType)
        {
            if(middlewareType == typeof(CommandMiddleware))
                return new CommandMiddleware(commands);

            return Activator.CreateInstance(middlewareType) as IMiddleware;
        }

        public void Release(IMiddleware middleware)
        {
        }
    }
}