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
using System.Collections.Generic;
using System.Threading.Tasks;
using DustInTheWind.ConsoleFramework;
using DustInTheWind.DirectoryCompare.Application.MiscellaneousArea.CompareSnapshots;
using DustInTheWind.DirectoryCompare.Domain.Comparison;
using DustInTheWind.DirectoryCompare.Infrastructure;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.MiscellaneousCommands
{
    [Command("compare")]
    [CommandDescription("Compares two snapshots.")]
    public class CompareSnapshotsCommandModel : ICommandModel
    {
        private readonly RequestBus requestBus;

        [CommandParameter(Index = 1)]
        public string Snapshot1Location { get; set; }

        [CommandParameter(Index = 2)]
        public string Snapshot2Location { get; set; }

        [CommandParameter(Index = 3, Optional = true)]
        public string ExportName { get; set; }

        public IReadOnlyList<string> OnlyInSnapshot1 { get; private set; }

        public IReadOnlyList<string> OnlyInSnapshot2 { get; private set; }

        public IReadOnlyList<ItemComparison> DifferentNames { get; private set; }

        public IReadOnlyList<ItemComparison> DifferentContent { get; private set; }

        public string ExportDirectoryPath { get; private set; }

        public CompareSnapshotsCommandModel(RequestBus requestBus)
        {
            this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
        }

        public async Task Execute(Arguments arguments)
        {
            CompareSnapshotsRequest request = CreateRequest();
            CompareSnapshotsResponse response = await requestBus.PlaceRequest<CompareSnapshotsRequest, CompareSnapshotsResponse>(request);

            OnlyInSnapshot1 = response.OnlyInSnapshot1;
            OnlyInSnapshot2 = response.OnlyInSnapshot2;
            DifferentNames = response.DifferentNames;
            DifferentContent = response.DifferentContent;
            ExportDirectoryPath = response.ExportDirectoryPath;
        }

        private CompareSnapshotsRequest CreateRequest()
        {
            CompareSnapshotsRequest request = new()
            {
                Snapshot1 = Snapshot1Location,
                Snapshot2 = Snapshot2Location
            };

            if (ExportName != null)
                request.ExportFileName = ExportName;

            return request;
        }
    }
}