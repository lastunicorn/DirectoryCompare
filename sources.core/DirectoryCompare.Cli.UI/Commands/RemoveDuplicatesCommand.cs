// DirectoryCompare
// Copyright (C) 2017-2019 Dust in the Wind
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

using DustInTheWind.ConsoleFramework;
using DustInTheWind.DirectoryCompare.Application.RemoveDuplicates;
using DustInTheWind.DirectoryCompare.Cli.UI.ResultExporters;
using DustInTheWind.DirectoryCompare.Domain.Comparison;
using MediatR;
using System;
using DustInTheWind.DirectoryCompare.Domain;

namespace DustInTheWind.DirectoryCompare.Cli.UI.Commands
{
    public class RemoveDuplicatesCommand : ICommand
    {
        private readonly IMediator mediator;

        public string Description => string.Empty;

        public RemoveDuplicatesCommand(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public void Execute(Arguments arguments)
        {
            RemoveDuplicatesRequest request = CreateRequest(arguments);
            mediator.Send(request).Wait();
        }

        private static RemoveDuplicatesRequest CreateRequest(Arguments arguments)
        {
            if (arguments.Count < 3)
                throw new Exception("Invalid command parameters.");

            SnapshotLocation snapshotLeft = arguments[0];
            SnapshotLocation snapshotRight = arguments[1];
            ComparisonSide fileToRemove = (ComparisonSide)Enum.Parse(typeof(ComparisonSide), arguments[2]);
            string destinationDirectory = arguments.Count >= 4
                ? arguments[3]
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