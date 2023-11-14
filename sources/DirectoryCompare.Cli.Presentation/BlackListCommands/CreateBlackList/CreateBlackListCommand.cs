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
using DustInTheWind.DirectoryCompare.Cli.Application.BlackListArea.CreateBlackList;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.BlackListCommands.CreateBlackList;
// Example:
// blacklist -c <blacklist-name> -p <pot-name>
// blacklist --create <blacklist-name> --pot <pot-name>

[NamedCommand("black-list", Description = "Creates a new black list in the specified pot.")]
internal class CreateBlackListCommand : IConsoleCommand
{
    private readonly RequestBus requestBus;

    [NamedParameter("create", ShortName = 'c')]
    public string BlackListName { get; set; }

    [NamedParameter("pot", ShortName = 'p')]
    public string PotName { get; set; }

    public CreateBlackListCommand(RequestBus requestBus)
    {
        this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
    }

    public async Task Execute()
    {
        CreateBlackListRequest request = new()
        {
            PotName = PotName,
            BlackListName = BlackListName
        };

        await requestBus.PlaceRequest(request);
    }
}