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
using DustInTheWind.DirectoryCompare.Cli.Application.UseCases.MiscellaneousArea.RemoveDuplicates;
using DustInTheWind.DirectoryCompare.DataStructures;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.MiscellaneousCommands;

[NamedCommand("remove-duplicates")]
public class RemoveDuplicatesCommand : IConsoleCommand
{
    private readonly RequestBus requestBus;

    [AnonymousParameter(Order = 1)]
    public SnapshotLocation LeftSnapshotLocation { get; set; }

    [AnonymousParameter(Order = 2)]
    public SnapshotLocation RightSnapshotLocation { get; set; }

    [AnonymousParameter(Order = 3)]
    public ComparisonSide FileToRemove { get; set; }

    [AnonymousParameter(Order = 4, IsOptional = true)]
    public string DestinationDirectory { get; set; }

    public RemoveDuplicatesCommand(RequestBus requestBus)
    {
        this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
    }

    public async Task Execute()
    {
        RemoveDuplicatesRequest request = new()
        {
            SnapshotLeft = LeftSnapshotLocation,
            SnapshotRight = RightSnapshotLocation,
            FileToRemove = FileToRemove,
            DestinationDirectory = DestinationDirectory
        };

        await requestBus.PlaceRequest(request);
    }
}