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
using DustInTheWind.DirectoryCompare.Application.SnapshotArea.ImportSnapshot;
using DustInTheWind.DirectoryCompare.Infrastructure;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.SnapshotCommands
{
    // Example:
    // snapshot -i <file-path> -p <pot-name>
    // snapshot --import <file-path> --pot <pot-name>

    public class ImportSnapshotCommand : ICommand
    {
        private readonly RequestBus requestBus;

        [CommandParameter(ShortName = "i", LongName = "import")]
        public string SnapshotFilePath { get; set; }

        [CommandParameter(ShortName = "p", LongName = "pot")]
        public string PotName { get; set; }

        public ImportSnapshotCommand(RequestBus requestBus)
        {
            this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
        }

        public async Task Execute(Arguments arguments)
        {
            ImportSnapshotRequest request = new()
            {
                FilePath = SnapshotFilePath,
                PotName = PotName
            };

            await requestBus.PlaceRequest(request);
        }
    }
}