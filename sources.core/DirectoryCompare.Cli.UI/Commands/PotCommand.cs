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
using DustInTheWind.DirectoryCompare.Application.UseCases.CreatePot;
using DustInTheWind.DirectoryCompare.Application.UseCases.GetPot;
using DustInTheWind.DirectoryCompare.Domain;
using DustInTheWind.DirectoryCompare.Domain.Utils;
using MediatR;
using System;
using System.Collections.Generic;

namespace DustInTheWind.DirectoryCompare.Cli.UI.Commands
{
    public class PotCommand : ICommand
    {
        private readonly IMediator mediator;

        public string Description => "Manages a pot that holds snapshots for a single path on disk.";

        public PotCommand(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public void Execute(Arguments arguments)
        {

            if (arguments.Count == 0)
            {
                GetPotRequest request = new GetPotRequest();
                List<Pot> pots = mediator.Send<List<Pot>>(request).Result;

                PotView potView = new PotView(pots);
                potView.Display();
            }
            else
            {
                CreatePotRequest request = CreateRequest(arguments);
                mediator.Send(request).Wait();
            }
        }

        private static CreatePotRequest CreateRequest(Arguments arguments)
        {
            return new CreatePotRequest
            {
                Name = arguments[0],
                Path = new DiskPath(arguments[1])
            };
        }
    }
}