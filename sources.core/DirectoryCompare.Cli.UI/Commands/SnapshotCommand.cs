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
using DustInTheWind.DirectoryCompare.Application.SnapshotArea.CreateSnapshot;
using DustInTheWind.DirectoryCompare.Application.SnapshotArea.DeleteSnapshot;
using DustInTheWind.DirectoryCompare.Application.SnapshotArea.ImportSnapshot;
using DustInTheWind.DirectoryCompare.Application.SnapshotArea.PresentSnapshot;
using DustInTheWind.DirectoryCompare.Cli.UI.Views;
using DustInTheWind.DirectoryCompare.Domain;
using DustInTheWind.DirectoryCompare.Domain.DiskAnalysis;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Infrastructure;

namespace DustInTheWind.DirectoryCompare.Cli.UI.Commands
{
    public class SnapshotCommand : ICommand
    {
        private readonly DirectoryCompareRequestBus requestBus;

        public string Key { get; } = "snapshot";

        public string Description => "Manages snapshots from a pot.";

        public SnapshotCommand(DirectoryCompareRequestBus requestBus)
        {
            this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
        }

        public void Execute(Arguments arguments)
        {
            Argument createArgument = arguments["c"];
            bool isCreate = !createArgument.IsEmpty;

            Argument deleteArgument = arguments["d"];
            bool isDelete = !deleteArgument.IsEmpty;

            Argument importArgument = arguments["i"];
            bool isImport = !importArgument.IsEmpty;

            if (isCreate)
            {
                // snapshot -c <pot-name>
                ExecuteCreate(createArgument);
            }
            else if (isDelete)
            {
                // snapshot -d <snapshot-location>
                ExecuteDelete(arguments, deleteArgument);
            }
            else if (isImport)
            {
                // snapshot -i <file-path> -p <pot-name>
                ExecuteImport(arguments, importArgument);
            }
            else
            {
                // snapshot <snapshot-location>
                ExecuteDisplay(arguments);
            }
        }

        private void ExecuteCreate(Argument createArgument)
        {
            CustomConsole.WriteLine("Creating new snapshot.");

            CreateSnapshotRequest request = new CreateSnapshotRequest
            {
                PotName = createArgument.HasValue
                    ? createArgument.Value
                    : null
            };

            IDiskAnalysisProgress pathAnalysisProgress = requestBus.SendAsync<CreateSnapshotRequest, IDiskAnalysisProgress>(request).Result;
            DisplayCreationProgress(pathAnalysisProgress);
        }

        private static void DisplayCreationProgress(IDiskAnalysisProgress pathAnalysisProgress)
        {
            CreateSnapshotView createSnapshotView = new CreateSnapshotView();
            pathAnalysisProgress.Progress += (sender, value) => createSnapshotView.DisplayProgress(value.Percentage);

            pathAnalysisProgress.WaitToEnd();
        }

        private void ExecuteDelete(Arguments arguments, Argument deleteArgument)
        {
            DeleteSnapshotRequest request = new DeleteSnapshotRequest
            {
                Location = deleteArgument.Value
            };

            requestBus.SendAsync(request).Wait();
        }

        private void ExecuteImport(Arguments arguments, Argument importArgument)
        {
            ImportSnapshotRequest request = new ImportSnapshotRequest
            {
                FilePath = importArgument.Value,
                PotName = arguments.GetStringValue("p")
            };
            requestBus.SendAsync(request).Wait();
        }

        private void ExecuteDisplay(Arguments arguments)
        {
            if (arguments.Values.Count <= 0 || arguments.Values[0].HasName)
                throw new Exception("Snapshot path must be provided.");

            SnapshotLocation snapshotLocation = arguments.Values[0].Value;

            PresentSnapshotRequest request = new PresentSnapshotRequest
            {
                Location = snapshotLocation
            };

            Snapshot snapshot = requestBus.SendAsync<PresentSnapshotRequest, Snapshot>(request).Result;

            SnapshotView snapshotView = new SnapshotView(snapshot);
            snapshotView.Display();
        }
    }
}