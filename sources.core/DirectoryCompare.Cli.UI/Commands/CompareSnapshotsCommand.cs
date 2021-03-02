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
using DustInTheWind.DirectoryCompare.Application.MiscelaneousArea.CompareSnapshots;
using DustInTheWind.DirectoryCompare.Cli.UI.Views;
using DustInTheWind.DirectoryCompare.Infrastructure;

namespace DustInTheWind.DirectoryCompare.Cli.UI.Commands
{
    public class CompareSnapshotsCommand : ICommand
    {
        private readonly DirectoryCompareRequestBus requestBus;

        public string Key { get; } = "compare";

        public string Description => "Compares two snapshots.";

        public CompareSnapshotsCommand(DirectoryCompareRequestBus requestBus)
        {
            this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
        }

        public void Execute(Arguments arguments)
        {
            CompareSnapshotsRequest request = CreateRequest(arguments);
            CompareSnapshotsResponse response = requestBus.SendAsync<CompareSnapshotsRequest, CompareSnapshotsResponse>(request).Result;

            bool exportedToFile = !string.IsNullOrEmpty(response.ExportDirectoryPath);

            if (exportedToFile)
            {
                CustomConsole.WriteLine("Results exported into directory: {0}", response.ExportDirectoryPath);
            }
            else
            {
                ConsoleComparisonView view = new ConsoleComparisonView
                {
                    Comparer = response.SnapshotComparer
                };
                view.Display();
            }
        }

        private static CompareSnapshotsRequest CreateRequest(Arguments arguments)
        {
            CompareSnapshotsRequest request = new CompareSnapshotsRequest
            {
                PotName1 = arguments.GetStringValue(0),
                PotName2 = arguments.GetStringValue(1)
            };

            bool exportToFile = arguments.Count >= 3;

            if (exportToFile)
                request.ExportFileName = arguments.GetStringValue(2);

            return request;
        }
    }
}