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

using DirectoryCompare.CliFramework;
using MediatR;
using System;
using DustInTheWind.DirectoryCompare.Application.UseCases.CreateSnapshot;

namespace DustInTheWind.DirectoryCompare.Cli.Commands
{
    internal class CreateSnapshotCommand : ICommand
    {
        private readonly IMediator mediator;

        public string Description => string.Empty;

        public CreateSnapshotCommand(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public void Execute(Arguments arguments)
        {
            CreateSnapshotRequest request = CreateRequest(arguments);
            mediator.Send(request).Wait();
        }

        private static CreateSnapshotRequest CreateRequest(Arguments arguments)
        {
            return new CreateSnapshotRequest
            {
                SourcePath = arguments[0],
                DestinationFilePath = arguments[1],
                BlackListFilePath = arguments[2]
            };
        }
    }
}