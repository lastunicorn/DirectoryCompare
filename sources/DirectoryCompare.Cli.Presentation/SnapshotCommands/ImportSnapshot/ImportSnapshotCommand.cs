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
using DustInTheWind.DirectoryCompare.Cli.Application.SnapshotArea.ImportSnapshot;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.SnapshotCommands.ImportSnapshot;
// Example:
// snapshot -i <file-path> -p <pot-name>
// snapshot --import <file-path> --pot <pot-name>

[NamedCommand("import-snapshot")]
[CommandOrder(8)]
public class ImportSnapshotCommand : IConsoleCommand
{
    private readonly RequestBus requestBus;

    [NamedParameter("path", ShortName = 'h')]
    public string SnapshotFilePath { get; set; }

    [NamedParameter("pot", ShortName = 'p')]
    public string PotName { get; set; }

    public ImportSnapshotCommand(RequestBus requestBus)
    {
        this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
    }

    public async Task Execute()
    {
        ImportSnapshotRequest request = new()
        {
            FilePath = SnapshotFilePath,
            PotName = PotName
        };

        await requestBus.PlaceRequest(request);
    }
}