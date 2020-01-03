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

using DustInTheWind.ConsoleFramework;
using DustInTheWind.DirectoryCompare.Domain.Utils;
using MediatR;
using System;
using DustInTheWind.DirectoryCompare.Application.AddBlackList;
using DustInTheWind.DirectoryCompare.Application.GetBlackList;
using DustInTheWind.DirectoryCompare.Application.RemoveBlackList;

namespace DustInTheWind.DirectoryCompare.Cli.UI.Commands
{
    public class BlackListCommand : ICommand
    {
        private readonly IMediator mediator;

        public BlackListCommand(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public string Description => string.Empty;

        public void Execute(Arguments arguments)
        {
            if (arguments.Count == 0)
                throw new Exception("No action is specified for the black-list command.");

            switch (arguments[0])
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
                PotName = arguments[1]
            };

            PathCollection blackList = mediator.Send<PathCollection>(request).Result;

            BlackListView blackListView = new BlackListView(blackList);
            blackListView.Display();
        }

        private void AddBlackList(Arguments arguments)
        {
            AddBlackListRequest request = new AddBlackListRequest
            {
                PotName = arguments[1],
                Path = arguments[2]
            };
            mediator.Send(request).Wait();
        }

        private void RemoveBlackList(Arguments arguments)
        {
            RemoveBlackListRequest request = new RemoveBlackListRequest
            {
                PotName = arguments[1],
                Path = arguments[2]
            };
            mediator.Send(request).Wait();
        }
    }
}