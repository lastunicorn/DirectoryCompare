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
using DustInTheWind.DirectoryCompare.Cli.Application.MiscellaneousArea.RemoveDuplicates;
using DustInTheWind.DirectoryCompare.DataStructures;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.MiscellaneousCommands.RemoveDuplicates;

[NamedCommand("remove-duplicates", Description = "Compares two snapshots or sub-parts of snapshots and remove the duplicate files from one of them.")]
public class RemoveDuplicatesCommand : IConsoleCommand
{
    private readonly RequestBus requestBus;

    [AnonymousParameter(Order = 1)]
    public SnapshotLocation LeftSnapshotLocation { get; set; }

    [AnonymousParameter(Order = 2)]
    public SnapshotLocation RightSnapshotLocation { get; set; }

    [NamedParameter("remove-from", ShortName = 'r', Description = "The side to be removed. Possible values: Left = 1, Right = 2.")]
    public ComparisonSidePm FileToRemove { get; set; }

    [NamedParameter("purgatory", ShortName = 'p')]
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
            FileToRemove = FileToRemove.ToBusiness(),
            PurgatoryDirectory = DestinationDirectory
        };

        await requestBus.PlaceRequest(request);
    }
}