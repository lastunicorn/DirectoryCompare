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
using DustInTheWind.DirectoryCompare.Cli.UI.Commands;
using MediatR;
using Ninject;

namespace DustInTheWind.DirectoryCompare.Cli.Setup
{
    internal static class CommandsSetup
    {
        public static CommandCollection Create(IKernel dependencyContainer)
        {
            IMediator mediator = dependencyContainer.Get<IMediator>();

            return new CommandCollection
            {
                { "pot", new PotCommand(mediator) },
                { "read", new CreateSnapshotCommand(mediator) },
                { "snapshot", new ViewSnapshotCommand(mediator) },
                { "compare", new CompareSnapshotsCommand(mediator) },
                { "find-duplicates", new FindDuplicatesCommand(mediator) },
                //{ "remove-duplicates", new RemoveDuplicatesCommand(mediator) }
                { "import", new ImportSnapshotCommand(mediator) },
                { "blacklist", new BlackListCommand(mediator) }
            };
        }
    }
}