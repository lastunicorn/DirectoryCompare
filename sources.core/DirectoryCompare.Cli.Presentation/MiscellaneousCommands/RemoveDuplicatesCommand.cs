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
    public class RemoveDuplicatesCommand : ICommand
    {
        private readonly RequestBus requestBus;

        public RemoveDuplicatesCommand(RequestBus requestBus)
        {
            this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
        }

        public async Task Execute(Arguments arguments)
        {
            RemoveDuplicatesRequest request = CreateRequest(arguments);
            await requestBus.PlaceRequest(request);
        }

        private static RemoveDuplicatesRequest CreateRequest(Arguments arguments)
        {
            if (arguments.Count < 3)
                throw new Exception("Invalid command parameters.");

            SnapshotLocation snapshotLeft = arguments.GetStringValue(0);
            SnapshotLocation snapshotRight = arguments.GetStringValue(1);
            ComparisonSide fileToRemove = (ComparisonSide)Enum.Parse(typeof(ComparisonSide), arguments.GetStringValue(2));
            string destinationDirectory = arguments.Count >= 4
                ? arguments.GetStringValue(3)
                : null;

            return new RemoveDuplicatesRequest
            {
                SnapshotLeft = snapshotLeft,
                SnapshotRight = snapshotRight,
                Exporter = new ConsoleRemoveDuplicatesExporter(),
                FileToRemove = fileToRemove,
                DestinationDirectory = destinationDirectory
            };
        }
    }
}