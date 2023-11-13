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
using DustInTheWind.DirectoryCompare.Cli.Application.BlackListArea.AddBlackPath;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.BlackListCommands.AddBlackListPath;
// Example:
// black-list -a <path> -p <pot-name> -b <black-list-name>
// black-list --add <path> --pot <pot-name> --black-list <black-list-name>

[NamedCommand("black-list")]
public class AddBlackListPathCommand : IConsoleCommand
{
    private readonly RequestBus requestBus;

    [NamedParameter("pot", ShortName = 'p')]
    public string PotName { get; set; }

    [NamedParameter("black-list", ShortName = 'b', IsOptional = true)]
    public string BlackListName { get; set; }

    [NamedParameter("add", ShortName = 'a')]
    public string Path { get; set; }

    public AddBlackListPathCommand(RequestBus requestBus)
    {
        this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
    }

    public async Task Execute()
    {
        AddBlackPathRequest request = new()
        {
            PotName = PotName,
            BlackList = BlackListName,
            Path = Path
        };

        await requestBus.PlaceRequest(request);
    }
}