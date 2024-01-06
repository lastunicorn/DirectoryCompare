// DirectoryCompare
// Copyright (C) 2017-2024 Dust in the Wind
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
using DustInTheWind.DirectoryCompare.Cli.Application.PotArea.PresentPots;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.PotCommands.DisplayPots;

// Example:
// pot

[NamedCommand("pots", Description = "Displays a list with all the existing pots.")]
[CommandOrder(4)]
public class DisplayPotsCommand : IConsoleCommand<PotsViewModel>
{
    private readonly RequestBus requestBus;

    [NamedParameter("size", ShortName = 's', Description = "When set, displays also the sizes of the pots.", IsOptional = true)]
    public bool DisplaySizes { get; set; }
    
    public DisplayPotsCommand(RequestBus requestBus)
    {
        this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
    }

    public async Task<PotsViewModel> Execute()
    {
        PresentPotsRequest request = new()
        {
            IncludeSizes = DisplaySizes
        };
        PresentPotsResponse response = await requestBus.PlaceRequest<PresentPotsRequest, PresentPotsResponse>(request);

        return new PotsViewModel
        {
            TotalSize = response.TotalSize,
            Pots = response.Pots
                .Select(x=> new PotViewModel(x))
                .ToList()
        };
    }
}