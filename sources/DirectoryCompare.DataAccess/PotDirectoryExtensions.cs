// DirectoryCompare
// Copyright (C) 2017-2024 Dust in the Wind
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

using DustInTheWind.DirectoryCompare.DataAccess.PotFiles;
using DustInTheWind.DirectoryCompare.DataAccess.PotFiles.PotInfoFileModel;
using DustInTheWind.DirectoryCompare.DataStructures;
using DustInTheWind.DirectoryCompare.Domain.PotModel;

namespace DustInTheWind.DirectoryCompare.DataAccess;

internal static class PotDirectoryExtensions
{
    public static Pot ToPot(this PotDirectory potDirectory)
    {
        JPotInfoFile jPotInfoFile = potDirectory.InfoFile;
        if (!jPotInfoFile.IsValid)
            return null;

        Pot pot = new()
        {
            Guid = potDirectory.PotGuid,
            Name = jPotInfoFile.Document.Name,
            Path = jPotInfoFile.Document.Path,
            Description = jPotInfoFile.Document.Description
        };

        if (potDirectory.InfoFile.Document?.Include != null)
        {
            IEnumerable<SnapshotPath> paths = potDirectory.InfoFile.Document.Include
                .Select(x => (SnapshotPath)x);

            pot.IncludedPaths.AddRange(paths);
        }

        return pot;
    }
}