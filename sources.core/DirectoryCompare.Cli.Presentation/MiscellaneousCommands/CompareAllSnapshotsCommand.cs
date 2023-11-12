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
using DustInTheWind.DirectoryCompare.Cli.Application.MiscellaneousArea.CompareAllSnapshots;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.MiscellaneousCommands;

[NamedCommand("compare-all", Description = "Compares all snapshots in a pot with the previous snapshot.")]
public class CompareAllSnapshotsCommand : IConsoleCommand
{
    private readonly RequestBus requestBus;

    [AnonymousParameter(Order = 1)]
    public string PotName { get; set; }

    [AnonymousParameter(Order = 2)]
    public string ExportName { get; set; }

    public CompareAllSnapshotsCommand(RequestBus requestBus)
    {
        this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
    }

    public async Task Execute()
    {
        CompareAllSnapshotsRequest request = new()
        {
            PotName = PotName,
            ExportName = ExportName
        };

        await requestBus.PlaceRequest<CompareAllSnapshotsRequest, CompareAllSnapshotsResponse>(request);
    }
}