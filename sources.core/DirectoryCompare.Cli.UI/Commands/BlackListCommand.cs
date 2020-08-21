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
using System.Collections.Generic;
using System.Linq;
using DustInTheWind.ConsoleFramework;
using DustInTheWind.DirectoryCompare.Application;
using DustInTheWind.DirectoryCompare.Application.AddBlackPath;
using DustInTheWind.DirectoryCompare.Application.GetBlackList;
using DustInTheWind.DirectoryCompare.Application.RemoveBlackPath;
using DustInTheWind.DirectoryCompare.Cli.UI.Views;
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
            Argument createBlacklistArgument = arguments["c"];
            bool isCreateBlacklist = !createBlacklistArgument.IsEmpty;

            Argument deleteBlacklistArgument = arguments["d"];
            bool isDeleteBlacklist = !deleteBlacklistArgument.IsEmpty;

            Argument addPathArgument = arguments["a"];
            bool isAddPath = !addPathArgument.IsEmpty;

            Argument removePathArgument = arguments["r"];
            bool isRemovePath = !removePathArgument.IsEmpty;

            if (isCreateBlacklist)
            {
                // blacklist -c <blacklist-name> -p <pot-name>
                ExecuteCreateBlacklist(arguments, createBlacklistArgument);
            }
            else if (isDeleteBlacklist)
            {
                // blacklist -d <blacklist-name> -p <pot-name>
                ExecuteDeleteBlacklist(arguments, deleteBlacklistArgument);
            }
            else if (isAddPath)
            {
                // blacklist -a <path> -p <pot-name> -b <blacklist-name>
                ExecuteAddPath(arguments, addPathArgument);
            }
            else if (isRemovePath)
            {
                // blacklist -r <path> -p <pot-name> -b <blacklist-name>
                ExecuteRemovePath(arguments, removePathArgument);
            }
            else
            {
                // blacklist <blacklist-name> -p <pot-name>
                ExecuteDisplay(arguments);
            }
        }

        private static void ExecuteCreateBlacklist(Arguments arguments, Argument createBlacklistArgument)
        {
            throw new NotImplementedException();
        }

        private static void ExecuteDeleteBlacklist(Arguments arguments, Argument deleteBlacklistArgument)
        {
            throw new NotImplementedException();
        }

        private void ExecuteAddPath(Arguments arguments, Argument addPathArgument)
        {
            AddBlackPathRequest request = new AddBlackPathRequest
            {
                PotName = arguments["p"].Value,
                BlackList = arguments["b"].Value,
                Path = addPathArgument.Value
            };
            requestBus.PlaceRequest(request).Wait();
        }

        private void ExecuteRemovePath(Arguments arguments, Argument removePathArgument)
        {
            RemoveBlackPathRequest request = new RemoveBlackPathRequest
            {
                PotName = arguments["p"].Value,
                BlackList = arguments["b"].Value,
                Path = removePathArgument.Value
            };
            requestBus.PlaceRequest(request).Wait();
        }

        private void ExecuteDisplay(Arguments arguments)
        {
            IEnumerable<Argument> anonymousArguments = arguments.GetAnonymousArguments();

            GetBlackListRequest request = new GetBlackListRequest
            {
                PotName = arguments["p"].Value,
                BlackList = anonymousArguments.FirstOrDefault().Value
            };

            PathCollection blackList = requestBus.PlaceRequest<GetBlackListRequest, PathCollection>(request).Result;

            BlackListView blackListView = new BlackListView(blackList);
            blackListView.Display();
        }
    }
}