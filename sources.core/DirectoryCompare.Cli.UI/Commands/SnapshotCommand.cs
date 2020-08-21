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
using DustInTheWind.DirectoryCompare.Application;
using DustInTheWind.DirectoryCompare.Application.CreateSnapshot;
using DustInTheWind.DirectoryCompare.Application.DeleteSnapshot;
using DustInTheWind.DirectoryCompare.Application.GetSnapshot;
using DustInTheWind.DirectoryCompare.Cli.UI.Views;
using DustInTheWind.DirectoryCompare.Domain;
using DustInTheWind.DirectoryCompare.Domain.Entities;

namespace DustInTheWind.DirectoryCompare.Cli.UI.Commands
{
    public class SnapshotCommand : ICommand
    {
        private readonly RequestBus requestBus;

        public string Description => "Manages snapshots from a pot.";

        public SnapshotCommand(RequestBus requestBus)
        {
            this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
        }

        public void Execute(Arguments arguments)
        {
            Argument createArgument = arguments["c"];
            bool isCreate = !createArgument.IsEmpty;

            Argument deleteArgument = arguments["d"];
            bool isDelete = !deleteArgument.IsEmpty;

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
            else
            {
                // snapshot <snapshot-location>
                ExecuteDisplay(arguments);
            }
        }

        private void ExecuteCreate(Argument createArgument)
        {
            CreateSnapshotRequest request = new CreateSnapshotRequest
            {
                PotName = createArgument.HasValue
                    ? createArgument.Value
                    : null
            };

            SnapshotProgress snapshotProgress = requestBus.PlaceRequest<CreateSnapshotRequest, SnapshotProgress>(request).Result;
            DisplayCreationProgress(snapshotProgress);
        }

        private static void DisplayCreationProgress(SnapshotProgress snapshotProgress)
        {
            CreateSnapshotView createSnapshotView = new CreateSnapshotView();
            snapshotProgress.ProgressChanged += (sender, value) => createSnapshotView.DisplayProgress(value);

            snapshotProgress.WaitToEnd();
        }

        private void ExecuteDelete(Arguments arguments, Argument deleteArgument)
        {
            DeleteSnapshotRequest request = new DeleteSnapshotRequest
            {
                Location = deleteArgument.Value
            };

            requestBus.PlaceRequest(request).Wait();
        }

        private void ExecuteDisplay(Arguments arguments)
        {
            SnapshotLocation snapshotLocation;

            if (arguments.Values.Count > 0 && !arguments.Values[0].HasName)
            {
                snapshotLocation = arguments.Values[0].Value;
            }
            else
            {
                throw new Exception("Snapshot path must be provided.");
            }

            GetSnapshotRequest request = new GetSnapshotRequest
            {
                Location = snapshotLocation
            };

            Snapshot snapshot = requestBus.PlaceRequest<GetSnapshotRequest, Snapshot>(request).Result;

            SnapshotView snapshotView = new SnapshotView(snapshot);
            snapshotView.Display();
        }
    }
}