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
using DustInTheWind.DirectoryCompare.Application.FindDuplicates;
using DustInTheWind.DirectoryCompare.Cli.UI.ResultExporters;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Cli.UI.Commands
{
    public class FindDuplicatesCommand : ICommand
    {
        private readonly IMediator mediator;

        public FindDuplicatesCommand(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public string Description => string.Empty;

        public void Execute(Arguments arguments)
        {
            FindDuplicatesRequest request = CreateRequest(arguments);
            mediator.Send(request).Wait();
        }

        private static FindDuplicatesRequest CreateRequest(Arguments arguments)
        {
            if (arguments.Count == 0)
                throw new Exception("Invalid command parameters.");

            string right;
            bool checkFilesExist;

            switch (arguments.Count)
            {
                case 1:
                    right = null;
                    checkFilesExist = false;
                    break;

                case 2:
                    bool success = bool.TryParse(arguments[1], out checkFilesExist);

                    if (success)
                    {
                        right = null;
                    }
                    else
                    {
                        right = arguments[1];
                        checkFilesExist = false;
                    }

                    break;

                case 3:
                    right = arguments[1];
                    bool.TryParse(arguments[2], out checkFilesExist);
                    break;

                default:
                    right = arguments[1];
                    bool.TryParse(arguments[2], out checkFilesExist);
                    break;
            }

            return new FindDuplicatesRequest
            {
                SnapshotLeft = arguments[0],
                SnapshotRight = right,
                Exporter = new ConsoleDuplicatesExporter(),
                CheckFilesExist = checkFilesExist
            };
        }
    }
}