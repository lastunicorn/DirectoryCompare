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
using DustInTheWind.DirectoryCompare.Application.FindDuplicates;
using DustInTheWind.DirectoryCompare.Cli.UI.ResultExporters;

namespace DustInTheWind.DirectoryCompare.Cli.UI.Commands
{
    public class FindDuplicatesCommand : ICommand
    {
        private readonly RequestBus requestBus;

        public FindDuplicatesCommand(RequestBus requestBus)
        {
            this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
        }

        public string Description => string.Empty;

        public void Execute(Arguments arguments)
        {
            FindDuplicatesRequest request = CreateRequest(arguments);
            requestBus.PlaceRequest(request).Wait();
        }

        private static FindDuplicatesRequest CreateRequest(Arguments arguments)
        {
            if (arguments.Count == 0)
                throw new Exception("Invalid command parameters.");

            Argument[] anonymousArguments = arguments.GetAnonymousArguments().ToArray();

            if (anonymousArguments.Length == 0)
                throw new Exception("Invalid command parameters.");

            string left = anonymousArguments.Length >= 1
                ? anonymousArguments[0].Value
                : null;

            string right = anonymousArguments.Length >= 2
                ? anonymousArguments[1].Value
                : null;

            bool checkFilesExist = arguments.GetBoolValue("x");

            return new FindDuplicatesRequest
            {
                SnapshotLeft = left,
                SnapshotRight = right,
                Exporter = new ConsoleDuplicatesExporter(),
                CheckFilesExist = checkFilesExist
            };
        }
    }
}