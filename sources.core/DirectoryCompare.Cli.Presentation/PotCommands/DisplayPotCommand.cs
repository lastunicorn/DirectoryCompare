// DirectoryCompare
// Copyright (C) 2017-2023 Dust in the Wind
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

using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.DirectoryCompare.Cli.Application;
using DustInTheWind.DirectoryCompare.Cli.Application.UseCases.PotArea.PresentPot;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.PotCommands;

// Example:
// pot <pot-name>

[NamedCommand("pot", Description = "Displays the details of the specified pot.")]
[CommandOrder(3)]
public class DisplayPotCommand : IConsoleCommand
{
    private readonly RequestBus requestBus;

    [AnonymousParameter(DisplayName = "pot name", Order = 1, IsOptional = true)]
    public string PotName { get; set; }

    public PotDto Pot { get; private set; }

    public DisplayPotCommand(RequestBus requestBus)
    {
        this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
    }

    public async Task Execute()
    {
        PresentPotRequest request = new()
        {
            PotName = PotName
        };

        PresentPotResponse response = await requestBus.PlaceRequest<PresentPotRequest, PresentPotResponse>(request);
        Pot = response.Pot;
    }
}