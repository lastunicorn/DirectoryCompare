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
using System.Collections.Generic;
using System.Linq;
using DustInTheWind.ConsoleFramework;
using DustInTheWind.DirectoryCompare.Application;
using DustInTheWind.DirectoryCompare.Application.CreatePot;
using DustInTheWind.DirectoryCompare.Application.DeletePot;
using DustInTheWind.DirectoryCompare.Application.GetPot;
using DustInTheWind.DirectoryCompare.Application.GetPots;
using DustInTheWind.DirectoryCompare.Domain.PotModel;
using DustInTheWind.DirectoryCompare.Domain.Utils;

namespace DustInTheWind.DirectoryCompare.Cli.UI.Commands
{
    public class PotCommand : ICommand
    {
        private readonly RequestBus requestBus;

        public string Description => "Manages a pot that holds snapshots for a single path on disk.";

        public PotCommand(RequestBus requestBus)
        {
            this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
        }

        public void Execute(Arguments arguments)
        {
            Argument createArgument = arguments.Values.FirstOrDefault(x => string.Equals(x.Name, "c", StringComparison.InvariantCultureIgnoreCase));
            bool isCreate = createArgument != null;

            Argument deleteArgument = arguments.Values.FirstOrDefault(x => string.Equals(x.Name, "d", StringComparison.InvariantCultureIgnoreCase));
            bool isDelete = deleteArgument != null;

            if (isCreate)
            {
                ExecuteCreate(arguments, createArgument);
            }
            else if (isDelete)
            {
                ExecuteDelete(deleteArgument);
            }
            else
            {
                bool hasArguments = arguments.Values.Any();

                if (hasArguments)
                    ExecuteDisplayOne(arguments);
                else
                    ExecuteDisplayAll();
            }
        }

        private void ExecuteCreate(Arguments arguments, Argument createArgument)
        {
            Argument pathArgument = arguments.Values.FirstOrDefault(x => string.Equals(x.Name, "p", StringComparison.InvariantCultureIgnoreCase));

            if (pathArgument == null)
                throw new Exception("Path must be provided");

            CreatePotRequest request = new CreatePotRequest
            {
                Name = createArgument.Value,
                Path = new DiskPath(pathArgument.Value)
            };

            requestBus.PlaceRequest(request).Wait();
        }

        private void ExecuteDelete(Argument deleteArgument)
        {
            DeletePotRequest request = new DeletePotRequest
            {
                PotName = deleteArgument.Value
            };

            requestBus.PlaceRequest(request).Wait();
        }

        private void ExecuteDisplayOne(Arguments arguments)
        {
            GetPotRequest request = new GetPotRequest
            {
                PotName = arguments[0]
            };
            Pot pot = requestBus.PlaceRequest<GetPotRequest, Pot>(request).Result;

            PotView potView = new PotView(pot);
            potView.Display();
        }

        private void ExecuteDisplayAll()
        {
            GetPotsRequest request = new GetPotsRequest();
            List<Pot> pots = requestBus.PlaceRequest<GetPotsRequest, List<Pot>>(request).Result;

            PotsView potsView = new PotsView(pots);
            potsView.Display();
        }
    }
}