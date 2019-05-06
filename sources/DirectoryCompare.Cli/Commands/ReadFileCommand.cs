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
using DirectoryCompare.CliFramework;
using DustInTheWind.DirectoryCompare.Application.TimePoint;
using DustInTheWind.DirectoryCompare.Entities;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Cli.Commands
{
    internal class ReadFileCommand : ICommand
    {
        private readonly IMediator mediator;

        public string Description => "Displays the content of a json hash files";

        public ReadFileCommand(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public void Execute(Arguments arguments)
        {
            GetTimePointRequest request = CreateRequest(arguments);
            HContainer container = mediator.Send(request).Result;

            ContainerView containerView = new ContainerView(container);
            containerView.Display();
        }

        private static GetTimePointRequest CreateRequest(Arguments arguments)
        {
            return new GetTimePointRequest
            {
                FilePath = arguments[0]
            };
        }
    }
}