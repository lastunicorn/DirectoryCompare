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
using DustInTheWind.DirectoryCompare.Application.BlackListArea.AddBlackPath;
using DustInTheWind.DirectoryCompare.Infrastructure;

namespace DustInTheWind.DirectoryCompare.Cli.UI.BlackListCommands
{
    // Example:
    // black-list -a <path> -p <pot-name> -b <black-list-name>
    // black-list --add <path> --pot <pot-name> --black-list <black-list-name>
    
    [Command("black-list")]
    public class AddBlackListPathCommand : ICommand
    {
        private readonly RequestBus requestBus;

        [CommandParameter(ShortName = "p", LongName = "pot")]
        public string PotName { get; set; }
        
        [CommandParameter(ShortName = "b", LongName = "black-list", Optional = true)]
        public string BlackListName { get; set; }
        
        [CommandParameter(ShortName = "a", LongName = "add")]
        public string Path { get; set; }
        
        public AddBlackListPathCommand(RequestBus requestBus)
        {
            this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
        }
        
        public async Task Execute(Arguments arguments)
        {
            AddBlackPathRequest request = new()
            {
                PotName = PotName,
                BlackList = BlackListName,
                Path = Path
            };
            
            await requestBus.PlaceRequest(request);
        }
    }
}