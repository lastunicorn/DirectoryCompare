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

using DustInTheWind.ConsoleFramework;
using DustInTheWind.DirectoryCompare.Cli.UI.Commands;
using Ninject;

namespace DustInTheWind.DirectoryCompare.Cli.Setup
{
    internal static class CommandsSetup
    {
        public static CommandCollection Create(IKernel dependencyContainer)
        {
            return new CommandCollection
            {
                { "pot", dependencyContainer.Get<PotCommand>() },
                { "read", dependencyContainer.Get<CreateSnapshotCommand>() },
                { "snapshot", dependencyContainer.Get<SnapshotCommand>() },
                { "compare", dependencyContainer.Get<CompareSnapshotsCommand>() },
                { "find-duplicates", dependencyContainer.Get<FindDuplicatesCommand>() },
                { "remove-duplicates", dependencyContainer.Get<RemoveDuplicatesCommand>() },
                { "import", dependencyContainer.Get<ImportSnapshotCommand>() },
                { "blacklist", dependencyContainer.Get<BlackListCommand>() }
            };
        }
    }
}