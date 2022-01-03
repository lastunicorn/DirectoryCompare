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
using System.Threading.Tasks;
using DustInTheWind.ConsoleFramework;
using DustInTheWind.DirectoryCompare.Application.PotArea.CreatePot;
using DustInTheWind.DirectoryCompare.Infrastructure;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.PotCommands
{
    // Examples:
    // pot -c <pot-name> -p <target-path>
    // pot --create <pot-name> --path <target-path>

    [Command(Name = "pot")]
    [CommandDescription("Creates a new pot for the specified disk path.")]
    public class CreatePotCommand : ICommand
    {
        private readonly RequestBus requestBus;

        [CommandParameter(ShortName = "c", LongName = "create")]
        public string PotName { get; set; }

        [CommandParameter(ShortName = "p", LongName = "path")]
        public string TargetPath { get; set; }

        public CreatePotCommand(RequestBus requestBus)
        {
            this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
        }

        public async Task Execute(Arguments arguments)
        {
            CreatePotRequest request = new()
            {
                Name = PotName,
                Path = TargetPath
            };
            
            await requestBus.PlaceRequest(request);
        }
    }
}