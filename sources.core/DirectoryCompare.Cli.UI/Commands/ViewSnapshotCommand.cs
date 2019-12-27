﻿// DirectoryCompare
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
using DustInTheWind.ConsoleFramework;
using DustInTheWind.DirectoryCompare.Application.UseCases.GetSnapshot;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Cli.UI.Commands
{
    public class ViewSnapshotCommand : ICommand
    {
        private readonly IMediator mediator;

        public string Description => "Displays the content of a json hash files";

        public ViewSnapshotCommand(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public void Execute(Arguments arguments)
        {
            GetSnapshotRequest request = CreateRequest(arguments);
            Snapshot snapshot = mediator.Send(request).Result;

            SnapshotView snapshotView = new SnapshotView(snapshot);
            snapshotView.Display();
        }

        private static GetSnapshotRequest CreateRequest(Arguments arguments)
        {
            return new GetSnapshotRequest
            {
                PotName = arguments[0]
            };
        }
    }
}