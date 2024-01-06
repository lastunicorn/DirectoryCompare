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
using DustInTheWind.DirectoryCompare.Cli.Application.PotArea.CreatePot;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.PotCommands.CreatePot;

// Examples:
// pot -c <pot-name> -p <target-path>
// pot --create <pot-name> --path <target-path>

[NamedCommand("create-pot", Description = "Creates a new pot for the specified disk path.")]
[CommandOrder(1)]
internal class CreatePotCommand : IConsoleCommand<CreatePotViewModel>
{
    private readonly RequestBus requestBus;

    [AnonymousParameter(Order = 1, Description = "The name of the pot to be created.")]
    public string PotName { get; set; }

    [AnonymousParameter(Order = 2, Description = "The path to be scanned and stored by the pot.")]
    public string TargetPath { get; set; }

    public CreatePotCommand(RequestBus requestBus)
    {
        this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
    }

    public async Task<CreatePotViewModel> Execute()
    {
        CreatePotRequest request = new()
        {
            Name = PotName,
            Path = TargetPath
        };

        CreatePotResponse response = await requestBus.PlaceRequest<CreatePotRequest, CreatePotResponse>(request);

        return new CreatePotViewModel
        {
            NewPotId = response.NewPotId
        };
    }
}