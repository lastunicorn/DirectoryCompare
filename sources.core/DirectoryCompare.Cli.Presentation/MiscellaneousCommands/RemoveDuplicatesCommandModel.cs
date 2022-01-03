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
using DustInTheWind.DirectoryCompare.Application.MiscellaneousArea.RemoveDuplicates;
using DustInTheWind.DirectoryCompare.Domain;
using DustInTheWind.DirectoryCompare.Domain.Comparison;
using DustInTheWind.DirectoryCompare.Infrastructure;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.MiscellaneousCommands
{
    [Command("remove-duplicates")]
    public class RemoveDuplicatesCommandModel : ICommandModel
    {
        private readonly RequestBus requestBus;

        [CommandParameter(Index = 1)]
        public SnapshotLocation LeftSnapshotLocation { get; set; }

        [CommandParameter(Index = 2)]
        public SnapshotLocation RightSnapshotLocation { get; set; }

        [CommandParameter(Index = 3)]
        public ComparisonSide FileToRemove { get; set; }

        [CommandParameter(Index = 4, Optional = true)]
        public string DestinationDirectory { get; set; }

        public RemoveDuplicatesCommandModel(RequestBus requestBus)
        {
            this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
        }

        public async Task Execute(Arguments arguments)
        {
            RemoveDuplicatesRequest request = new()
            {
                SnapshotLeft = LeftSnapshotLocation,
                SnapshotRight = RightSnapshotLocation,
                Exporter = new ConsoleRemoveDuplicatesExporter(),
                FileToRemove = FileToRemove,
                DestinationDirectory = DestinationDirectory
            };

            await requestBus.PlaceRequest(request);
        }
    }
}