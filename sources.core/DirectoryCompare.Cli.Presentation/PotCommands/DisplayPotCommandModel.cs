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
using DustInTheWind.ConsoleFramework;
using DustInTheWind.DirectoryCompare.Application.PotArea.PresentPot;
using DustInTheWind.DirectoryCompare.Domain.PotModel;
using DustInTheWind.DirectoryCompare.Infrastructure;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.PotCommands
{
    // Example:
    // pot <pot-name>

    [Command(Name = "pot")]
    [CommandDescription("Displays the details of the specified pot.")]
    public class DisplayPotCommandModel : ICommandModel
    {
        private readonly RequestBus requestBus;

        [CommandParameter(Index = 1)]
        public string PotName { get; set; }

        public Pot Pot { get; private set; }

        public DisplayPotCommandModel(RequestBus requestBus)
        {
            this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
        }

        public async Task Execute(Arguments arguments)
        {
            PresentPotRequest request = new()
            {
                PotName = PotName
            };

            Pot = await requestBus.PlaceRequest<PresentPotRequest, Pot>(request);
        }
    }
}