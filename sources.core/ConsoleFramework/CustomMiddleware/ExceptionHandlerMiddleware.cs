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
using System.Threading.Tasks;
using DustInTheWind.ConsoleFramework.AppBuilder;
using DustInTheWind.ConsoleFramework.Logging;
using DustInTheWind.ConsoleTools;

namespace DustInTheWind.ConsoleFramework.CustomMiddleware
{
    internal class ExceptionHandlerMiddleware : IConsoleMiddleware
    {
        private readonly IProjectLogger logger;

        public ExceptionHandlerMiddleware(IProjectLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(ConsoleRequestContext context, RequestDelegate next)
        {
            try
            {
                if (next != null)
                    await next.Invoke(context);
            }
            catch (Exception ex)
            {
                CustomConsole.WriteLineError(ex.Message);
                logger.Error(ex.ToString());

                await Task.FromResult(null as object);
            }
        }
    }
}