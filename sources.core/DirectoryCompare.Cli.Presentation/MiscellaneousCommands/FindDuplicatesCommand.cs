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

using System;
using System.Threading.Tasks;
using DustInTheWind.ConsoleFramework;
using DustInTheWind.DirectoryCompare.Application.MiscellaneousArea.FindDuplicates;
using DustInTheWind.DirectoryCompare.Domain;
using DustInTheWind.DirectoryCompare.Domain.Comparison;
using DustInTheWind.DirectoryCompare.Domain.Utils;
using DustInTheWind.DirectoryCompare.Infrastructure;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.MiscellaneousCommands
{
    // Example:
    // find-duplicates snap1 snap2 -x
    // find-duplicates snap1 snap2 --check-files-existence
    // find-duplicates snap1 -x
    // find-duplicates snap1 --check-files-existence

    [Command("find-duplicates")]
    internal class FindDuplicatesCommand : ICommand
    {
        private readonly RequestBus requestBus;
        private FileDuplicates fileDuplicates;

        [CommandParameter(Index = 1)]
        public string Snapshot1Location { get; set; }

        [CommandParameter(Index = 2, Optional = true)]
        public string Snapshot2Location { get; set; }

        [CommandParameter(ShortName = "x", LongName = "check-files-existence", Optional = true)]
        public bool CheckFilesExistence { get; set; }

        public FileDuplicatesViewModel FileDuplicates { get; private set; }

        public int DuplicateCount => fileDuplicates?.DuplicateCount ?? 0;

        public DataSize TotalSize => fileDuplicates?.TotalSize ?? DataSize.Zero;

        public FindDuplicatesCommand(RequestBus requestBus)
        {
            this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
        }

        public async Task Execute(Arguments arguments)
        {
            FindDuplicatesRequest request = new()
            {
                SnapshotLeft = Snapshot1Location,
                SnapshotRight = Snapshot2Location,
                CheckFilesExistence = CheckFilesExistence
            };
            
            fileDuplicates = await requestBus.PlaceRequest<FindDuplicatesRequest, FileDuplicates>(request);

            FileDuplicates = new FileDuplicatesViewModel(fileDuplicates);
        }
    }
}