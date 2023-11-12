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
using DustInTheWind.DirectoryCompare.Cli.Application.UseCases.SnapshotArea.PresentSnapshot;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.SnapshotCommands;

// Example:
// snapshot <snapshot-location>

[NamedCommand("snapshot")]
[CommandOrder(7)]
public class DisplaySnapshotCommand : IConsoleCommand
{
    private readonly RequestBus requestBus;

    [AnonymousParameter(Order = 1)]
    public string SnapshotLocation { get; set; }

    public PresentSnapshotResponse Response { get; private set; }

    public DisplaySnapshotCommand(RequestBus requestBus)
    {
        this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
    }

    public async Task Execute()
    {
        PresentSnapshotRequest request = new()
        {
            Location = SnapshotLocation
        };

        Response = await requestBus.PlaceRequest<PresentSnapshotRequest, PresentSnapshotResponse>(request);
    }
}