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
using DustInTheWind.DirectoryCompare.Application;
using DustInTheWind.DirectoryCompare.Application.AddBlackList;
using DustInTheWind.DirectoryCompare.Application.GetBlackList;
using DustInTheWind.DirectoryCompare.Application.RemoveBlackList;
using DustInTheWind.DirectoryCompare.Domain.Utils;

namespace DustInTheWind.DirectoryCompare.Cli.UI.Commands
{
    public class BlackListCommand : ICommand
    {
        private readonly RequestBus requestBus;

        public BlackListCommand(RequestBus requestBus)
        {
            this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
        }

        public string Description => string.Empty;

        public void Execute(Arguments arguments)
        {
            if (arguments.Count == 0)
                throw new Exception("No action is specified for the black-list command.");

            switch (arguments.GetStringValue(0))
            {
                case "get":
                    DisplayBlackList(arguments);
                    break;

                case "add":
                    AddBlackList(arguments);
                    break;


                case "remove":
                    RemoveBlackList(arguments);
                    break;

                default:
                    throw new Exception("Invalid action for black list command.");
            }
        }

        private void DisplayBlackList(Arguments arguments)
        {
            GetBlackListRequest request = new GetBlackListRequest
            {
                PotName = arguments.GetStringValue(1)
            };

            PathCollection blackList = requestBus.PlaceRequest<GetBlackListRequest, PathCollection>(request).Result;

            BlackListView blackListView = new BlackListView(blackList);
            blackListView.Display();
        }

        private void AddBlackList(Arguments arguments)
        {
            AddBlackListRequest request = new AddBlackListRequest
            {
                PotName = arguments.GetStringValue(1),
                Path = arguments.GetStringValue(2)
            };
            requestBus.PlaceRequest(request).Wait();
        }

        private void RemoveBlackList(Arguments arguments)
        {
            RemoveBlackListRequest request = new RemoveBlackListRequest
            {
                PotName = arguments.GetStringValue(1),
                Path = arguments.GetStringValue(2)
            };
            requestBus.PlaceRequest(request).Wait();
        }
    }
}