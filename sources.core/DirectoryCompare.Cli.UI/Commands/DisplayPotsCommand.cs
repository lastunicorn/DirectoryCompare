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
using DustInTheWind.ConsoleFramework;
using DustInTheWind.DirectoryCompare.Application.PotArea.PresentPots;
using DustInTheWind.DirectoryCompare.Cli.UI.Views;
using DustInTheWind.DirectoryCompare.Domain.PotModel;
using DustInTheWind.DirectoryCompare.Infrastructure;

namespace DustInTheWind.DirectoryCompare.Cli.UI.Commands
{
    // Example:
    // pot

    [Command(Name = "pot")]
    [CommandDescription("Displays a list with all the existing pot.")]
    public class DisplayPotsCommand : ICommand
    {
        private readonly RequestBus requestBus;

        public DisplayPotsCommand(RequestBus requestBus)
        {
            this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
        }

        public void Execute(Arguments arguments)
        {
            PresentPotsRequest request = new();
            List<Pot> pots = requestBus.PlaceRequest<PresentPotsRequest, List<Pot>>(request).Result;

            PotsView potsView = new(pots);
            potsView.Display();
        }
    }
}