// DirectoryCompare
// Copyright (C) 2017-2020 Dust in the Wind
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
using DustInTheWind.DirectoryCompare.Cli.Application.SnapshotArea.DeleteSnapshot;
using DustInTheWind.DirectoryCompare.Infrastructure;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.SnapshotCommands;

// Example:
// snapshot -d <snapshot-location>
// snapshot --delete <snapshot-location>

[NamedCommand("delete-snapshot")]
[CommandOrder(6)]
public class DeleteSnapshotCommand : IConsoleCommand
{
    private readonly RequestBus requestBus;

    [NamedParameter("location", ShortName = 'l')]
    public string SnapshotLocation { get; set; }

    public DeleteSnapshotCommand(RequestBus requestBus)
    {
        this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
    }

    public async Task Execute()
    {
        DeleteSnapshotRequest request = new()
        {
            Location = SnapshotLocation
        };

        await requestBus.PlaceRequest(request);
    }
}