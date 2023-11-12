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
using DustInTheWind.DirectoryCompare.Cli.Application.UseCases.PotArea.PresentPots;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.PotCommands;

// Example:
// pot

[NamedCommand("pots", Description = "Displays a list with all the existing pots.")]
[CommandOrder(4)]
public class DisplayPotsCommand : IConsoleCommand
{
    private readonly RequestBus requestBus;

    public List<PotDto> Pots { get; private set; }

    public DisplayPotsCommand(RequestBus requestBus)
    {
        this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
    }

    public async Task Execute()
    {
        PresentPotsRequest request = new();
        PresentPotsResponse response = await requestBus.PlaceRequest<PresentPotsRequest, PresentPotsResponse>(request);
        Pots = response.Pots;
    }
}