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
using DustInTheWind.DirectoryCompare.Cli.Application.MiscellaneousArea.CompareSnapshots;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.MiscellaneousCommands.CompareSnapshots;

[NamedCommand("compare", Description = "Compares two snapshots.")]
[CommandOrder(10)]
public class CompareSnapshotsCommand : IConsoleCommand<CompareViewModel>
{
    private readonly RequestBus requestBus;

    [AnonymousParameter(Order = 1, Description = "The location of the first snapshot to be compared. The location includes the pot, snapshot and path.")]
    public string Snapshot1Location { get; set; }

    [AnonymousParameter(Order = 2, Description = "The location of the first snapshot to be compared. The location includes the pot, snapshot and path.")]
    public string Snapshot2Location { get; set; }

    [AnonymousParameter(Order = 3, IsOptional = true, Description = "If a name is provided, the results are exported in a directory on disk.")]
    public string ExportName { get; set; }

    public CompareSnapshotsCommand(RequestBus requestBus)
    {
        this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
    }

    public async Task<CompareViewModel> Execute()
    {
        CompareSnapshotsRequest request = CreateRequest();
        CompareSnapshotsResponse response = await requestBus.PlaceRequest<CompareSnapshotsRequest, CompareSnapshotsResponse>(request);

        return new CompareViewModel(response);
    }

    private CompareSnapshotsRequest CreateRequest()
    {
        CompareSnapshotsRequest request = new()
        {
            Snapshot1 = Snapshot1Location,
            Snapshot2 = Snapshot2Location
        };

        if (ExportName != null)
            request.ExportFileName = ExportName;

        return request;
    }
}