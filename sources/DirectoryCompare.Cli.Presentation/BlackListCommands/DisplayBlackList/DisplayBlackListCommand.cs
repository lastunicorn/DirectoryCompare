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
using DustInTheWind.DirectoryCompare.Cli.Application.BlackListArea.PresentBlackList;
using DustInTheWind.DirectoryCompare.DataStructures;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.BlackListCommands.DisplayBlackList;
// Example:
// black-list <black-list-name> -p <pot-name>
// black-list <black-list-name> --pot <pot-name>

[NamedCommand("black-list", Description = "Displays the list of paths from a black list.")]
public class DisplayBlackListCommand : IConsoleCommand
{
    private readonly RequestBus requestBus;

    [AnonymousParameter(Order = 1)]
    public string BlackListName { get; set; }

    [NamedParameter("pot", ShortName = 'p')]
    public string PotName { get; set; }

    public DiskPathCollection BlackList { get; private set; }

    public DisplayBlackListCommand(RequestBus requestBus)
    {
        this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
    }

    public async Task Execute()
    {
        PresentBlackListRequest request = new()
        {
            PotName = PotName,
            BlackListName = BlackListName
        };

        BlackList = await requestBus.PlaceRequest<PresentBlackListRequest, DiskPathCollection>(request);
    }
}