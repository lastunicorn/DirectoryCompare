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
using System.Linq;
using DustInTheWind.ConsoleFramework;
using DustInTheWind.DirectoryCompare.Application;
using DustInTheWind.DirectoryCompare.Application.CreateSnapshot;
using DustInTheWind.DirectoryCompare.Application.GetSnapshot;
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
            Argument createArgument = arguments.Values.FirstOrDefault(x => string.Equals(x.Name, "c", StringComparison.InvariantCultureIgnoreCase));
            bool isCreate = !createArgument.IsEmpty;

            Argument deleteArgument = arguments.Values.FirstOrDefault(x => string.Equals(x.Name, "d", StringComparison.InvariantCultureIgnoreCase));
            bool isDelete = !deleteArgument.IsEmpty;

            if (isCreate)
            {
                // snapshot -c -p <pot-name>
                // snapshot -c <pot-name>
                ExecuteCreate(arguments, createArgument);
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

        private void ExecuteCreate(Arguments arguments, Argument createArgument)
        {
            string potName = GetPotNameFromCreateArgument(arguments, createArgument);

            CreateSnapshotRequest request = new CreateSnapshotRequest
            {
                PotName = potName
            };

            SnapshotProgress snapshotProgress = requestBus.PlaceRequest<CreateSnapshotRequest, SnapshotProgress>(request).Result;

            CreateSnapshotView createSnapshotView = new CreateSnapshotView();
            snapshotProgress.ProgressChanged += (sender, value) => createSnapshotView.DisplayProgress(value);

            snapshotProgress.WaitToEnd();
        }

        private static string GetPotNameFromCreateArgument(Arguments arguments, Argument createArgument)
        {
            if (!string.IsNullOrEmpty(createArgument.Value))
                return createArgument.Value;

            Argument potArgument = arguments.Values.FirstOrDefault(x => string.Equals(x.Name, "p", StringComparison.InvariantCultureIgnoreCase));

            if (potArgument.IsEmpty)
                throw new Exception("Pot name must be provided.");

            return potArgument.Value;
        }

        private static void ExecuteDelete(Arguments arguments, Argument deleteArgument)
        {
            throw new NotImplementedException();
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

                Argument potArgument = arguments.Values.FirstOrDefault(x => string.Equals(x.Name, "p", StringComparison.InvariantCultureIgnoreCase));

                if (potArgument.IsEmpty)
                    throw new Exception("Pot name must be provided.");

                snapshotLocation = potArgument.Value;
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