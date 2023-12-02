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
using DustInTheWind.DirectoryCompare.Cli.Application.MiscellaneousArea.FindDuplicates;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.MiscellaneousCommands.FindDuplicates;
// Example:
// duplicates snap1 snap2 -x
// duplicates snap1 snap2 --check-files-existence
// duplicates snap1 -x
// duplicates snap1 --check-files-existence

[NamedCommand("duplicates", Description = "Search and display all the duplicate files in a single snapshot or between two snapshots.")]
[CommandOrder(11)]
internal class FindDuplicatesCommand : IConsoleCommand
{
    private readonly RequestBus requestBus;

    [AnonymousParameter(Order = 1, Description = "The location of the first snapshot to compare. Location structure: '<pot>~<snapshot>:<path>'.")]
    public string Snapshot1Location { get; set; }

    [AnonymousParameter(Order = 2, IsOptional = true, Description = "The location of the second snapshot to compare. Location structure: '<pot>~<snapshot>:<path>'.")]
    public string Snapshot2Location { get; set; }

    [NamedParameter("check-exist", ShortName = 'x', IsOptional = true, Description = "If specified, the files that does not actually exist on disk are not displayed.")]
    public bool CheckFilesExistence { get; set; }

    public FindDuplicatesCommand(RequestBus requestBus)
    {
        this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
    }

    public async Task Execute()
    {
        FindDuplicatesRequest request = new()
        {
            SnapshotLeft = Snapshot1Location,
            SnapshotRight = Snapshot2Location,
            CheckFilesExistence = CheckFilesExistence
        };

        await requestBus.PlaceRequest(request);
    }
}