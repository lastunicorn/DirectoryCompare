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
using DustInTheWind.DirectoryCompare.Cli.Application.BlackListArea.DeleteBlackList;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.BlackListCommands.DeleteBlackList;
// Examples:
// black-list -d <black-list-name> -p <pot-name>
// black-list --delete <black-list-name> --pot <pot-name>

[NamedCommand("black-list", Description = "Deletes the black list with the specified name.")]
public class DeleteBlackListCommand : IConsoleCommand
{
    private readonly RequestBus requestBus;

    [NamedParameter("delete", ShortName = 'd')]
    public string BlackListName { get; set; }

    [NamedParameter("pot", ShortName = 'p')]
    public string PotName { get; set; }

    public DeleteBlackListCommand(RequestBus requestBus)
    {
        this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
    }

    public async Task Execute()
    {
        DeleteBlackListRequest request = new()
        {
            PotName = PotName,
            BlackListName = BlackListName
        };

        await requestBus.PlaceRequest(request);
    }
}