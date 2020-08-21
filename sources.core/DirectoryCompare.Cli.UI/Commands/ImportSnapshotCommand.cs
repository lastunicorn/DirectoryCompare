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
using DustInTheWind.DirectoryCompare.Application.ImportSnapshot;

namespace DustInTheWind.DirectoryCompare.Cli.UI.Commands
{
    public class ImportSnapshotCommand : ICommand
    {
        private readonly RequestBus requestBus;

        public ImportSnapshotCommand(RequestBus requestBus)
        {
            this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
        }

        public string Description => string.Empty;

        public void Execute(Arguments arguments)
        {
            ImportSnapshotRequest request = CreateRequest(arguments);
            requestBus.PlaceRequest(request).Wait();
        }

        private static ImportSnapshotRequest CreateRequest(Arguments arguments)
        {
            return new ImportSnapshotRequest
            {
                FilePath = arguments.GetStringValue(0),
                PotName = arguments.GetStringValue(1)
            };
        }
    }
}