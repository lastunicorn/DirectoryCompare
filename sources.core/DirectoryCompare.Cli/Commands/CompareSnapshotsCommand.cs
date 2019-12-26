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

using System;
using DustInTheWind.ConsoleFramework;
using DustInTheWind.DirectoryCompare.Application;
using DustInTheWind.DirectoryCompare.Application.UseCases.CompareSnapshots;
using DustInTheWind.DirectoryCompare.Cli.ResultExporters;
using DustInTheWind.DirectoryCompare.Domain.Comparison;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Cli.Commands
{
    internal class CompareSnapshotsCommand : ICommand
    {
        private readonly IMediator mediator;

        public string Description => "Compares two json hash files.";

        public CompareSnapshotsCommand(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public void Execute(Arguments arguments)
        {
            CompareSnapshotsRequest request = CreateRequest(arguments);
            SnapshotComparer snapshotComparer = mediator.Send(request).Result;

            bool exportToFile = arguments.Count >= 3;

            if (exportToFile)
            {
                FileComparisonExporter exporter = new FileComparisonExporter { ResultsDirectory = arguments[2] };
                exporter.Export(snapshotComparer);
            }
            else
            {
                ConsoleComparisonExporter exporter = new ConsoleComparisonExporter();
                exporter.Export(snapshotComparer);
            }
        }

        private static CompareSnapshotsRequest CreateRequest(Arguments arguments)
        {
            return new CompareSnapshotsRequest
            {
                Path1 = arguments[0],
                Path2 = arguments[1]
            };
        }
    }
}