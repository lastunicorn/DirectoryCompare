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
using DustInTheWind.DirectoryCompare.Application.CompareSnapshots;
using DustInTheWind.DirectoryCompare.Cli.UI.ResultExporters;
using DustInTheWind.DirectoryCompare.Domain.Comparison;

namespace DustInTheWind.DirectoryCompare.Cli.UI.Commands
{
    public class CompareSnapshotsCommand : ICommand
    {
        private readonly RequestBus requestBus;

        public string Description => "Compares two snapshots.";

        public CompareSnapshotsCommand(RequestBus requestBus)
        {
            this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
        }

        public void Execute(Arguments arguments)
        {
            CompareSnapshotsRequest request = CreateRequest(arguments);
            SnapshotComparer snapshotComparer = requestBus.PlaceRequest<CompareSnapshotsRequest, SnapshotComparer>(request).Result;

            bool exportToFile = arguments.Count >= 3;

            if (exportToFile)
            {
                FileComparisonExporter exporter = new FileComparisonExporter { ResultsDirectory = arguments.GetStringValue(2) };
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
                PotName1 = arguments.GetStringValue(0),
                PotName2 = arguments.GetStringValue(1)
            };
        }
    }
}