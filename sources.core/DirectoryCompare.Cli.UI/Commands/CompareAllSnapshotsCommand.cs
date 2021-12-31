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
using DustInTheWind.ConsoleFramework;
using DustInTheWind.ConsoleTools;
using DustInTheWind.DirectoryCompare.Application.MiscellaneousArea.CompareAllSnapshots;
using DustInTheWind.DirectoryCompare.Application.MiscellaneousArea.CompareSnapshots;
using DustInTheWind.DirectoryCompare.Cli.UI.Views;
using DustInTheWind.DirectoryCompare.Infrastructure;

namespace DustInTheWind.DirectoryCompare.Cli.UI.Commands
{
    public class CompareAllSnapshotsCommand : ICommand
    {
        private readonly RequestBus requestBus;

        public string Key { get; } = "compare-all";

        public string Description => "Compares all snapshots in a pot with the previous snapshot.";

        public CompareAllSnapshotsCommand(RequestBus requestBus)
        {
            this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
        }

        public void Execute(Arguments arguments)
        {
            CompareAllSnapshotsRequest request = CreateRequest(arguments);
            CompareAllSnapshotsResponse response = requestBus.PlaceRequest<CompareAllSnapshotsRequest, CompareAllSnapshotsResponse>(request).Result;

            CustomConsole.WriteLine("Results exported successfully");
        }

        private static CompareAllSnapshotsRequest CreateRequest(Arguments arguments)
        {
            CompareAllSnapshotsRequest request = new CompareAllSnapshotsRequest
            {
                PotName = arguments.GetStringValue(0)
            };

            bool exportToFile = arguments.Count >= 2;

            if (exportToFile)
                request.ExportName = arguments.GetStringValue(1);

            return request;
        }
    }
}