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
using DustInTheWind.DirectoryCompare.Cli.Application.PotArea.DeletePot;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.PotCommands.DeletePot;

// Examples:
// pot -d <pot-name>
// pot --delete <pot-name>

[NamedCommand("delete-pot", Description = "Deletes the pot with the specified name.")]
[CommandOrder(2)]
public class DeletePotCommand : IConsoleCommand
{
    private readonly RequestBus requestBus;

    [AnonymousParameter(Order = 1, Description = "The name or id of the pot to be deleted.")]
    public string PotName { get; set; }

    public DeletePotCommand(RequestBus requestBus)
    {
        this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
    }

    public async Task Execute()
    {
        DeletePotRequest request = new()
        {
            PotName = PotName
        };

        await requestBus.PlaceRequest(request);
    }
}