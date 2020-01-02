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

using DustInTheWind.ConsoleFramework;
using DustInTheWind.DirectoryCompare.Application.UseCases.CreateSnapshot;
using MediatR;
using System;

namespace DustInTheWind.DirectoryCompare.Cli.UI.Commands
{
    public class CreateSnapshotCommand : ICommand
    {
        private readonly IMediator mediator;
        private CreateSnapshotView createSnapshotView;

        public string Description => "Creates a new snapshot in a specific pot.";

        public CreateSnapshotCommand(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public void Execute(Arguments arguments)
        {
            createSnapshotView = new CreateSnapshotView();

            CreateSnapshotRequest request = CreateRequest(arguments);
            mediator.Send(request).Wait();
        }

        private  CreateSnapshotRequest CreateRequest(Arguments arguments)
        {
            return new CreateSnapshotRequest
            {
                PotName = arguments[0],
                Progress = new Progress<float>(value => createSnapshotView.DisplayProgress(value))
            };
        }
    }
}