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
using DustInTheWind.DirectoryCompare.Cli.UI.Views;

namespace DustInTheWind.DirectoryCompare.Cli.UI.Commands
{
    public class CreateSnapshotCommand : ICommand
    {
        private readonly RequestBus requestBus;
        private CreateSnapshotView createSnapshotView;

        public string Key { get; } = "read";

        public string Description => "Creates a new snapshot in a specific pot.";

        public CreateSnapshotCommand(RequestBus requestBus)
        {
            this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
        }

        public void Execute(Arguments arguments)
        {
            createSnapshotView = new CreateSnapshotView();

            CreateSnapshotRequest request = CreateRequest(arguments);

            SnapshotProgress snapshotProgress = requestBus.PlaceRequest<CreateSnapshotRequest, SnapshotProgress>(request).Result;
            snapshotProgress.ProgressChanged += (sender, value) => createSnapshotView.DisplayProgress(value);

            snapshotProgress.WaitToEnd();
        }

        private static CreateSnapshotRequest CreateRequest(Arguments arguments)
        {
            return new CreateSnapshotRequest
            {
                PotName = arguments.GetStringValue(0)
            };
        }
    }
}