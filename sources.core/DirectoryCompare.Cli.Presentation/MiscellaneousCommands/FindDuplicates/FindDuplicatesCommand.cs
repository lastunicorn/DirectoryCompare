﻿// DirectoryCompare
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
// find-duplicates snap1 snap2 -x
// find-duplicates snap1 snap2 --check-files-existence
// find-duplicates snap1 -x
// find-duplicates snap1 --check-files-existence

[NamedCommand("find-duplicates")]
[CommandOrder(11)]
internal class FindDuplicatesCommand : IConsoleCommand
{
    private readonly RequestBus requestBus;

    [AnonymousParameter(Order = 1)]
    public string Snapshot1Location { get; set; }

    [AnonymousParameter(Order = 2, IsOptional = true)]
    public string Snapshot2Location { get; set; }

    [NamedParameter("check-files-existence", ShortName = 'x', IsOptional = true)]
    public bool CheckFilesExistence { get; set; }

    public FileDuplicatesViewModel FileDuplicates { get; private set; }

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

        FindDuplicatesResponse response = await requestBus.PlaceRequest<FindDuplicatesRequest, FindDuplicatesResponse>(request);

        FileDuplicates = new FileDuplicatesViewModel(response.DuplicatePairs);
    }
}