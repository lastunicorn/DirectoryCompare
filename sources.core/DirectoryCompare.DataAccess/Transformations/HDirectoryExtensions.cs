// DirectoryCompare
// Copyright (C) 2017-2023 Dust in the Wind
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

using DustInTheWind.DirectoryCompare.DataAccess.PotFiles.SnapshotFileModel;
using DustInTheWind.DirectoryCompare.Domain.Entities;

namespace DustInTheWind.DirectoryCompare.DataAccess.Transformations;

internal static class HDirectoryExtensions
{
    public static JDirectory ToJDirectory(this HDirectory directory)
    {
        if (directory == null) throw new ArgumentNullException(nameof(directory));

        return new JDirectory
        {
            Name = directory.Name,
            Directories = directory.Directories?
                .Select(x => x.ToJDirectory())
                .ToList(),
            Files = directory.Files?
                .Select(x => x.ToJFile())
                .ToList()
        };
    }
}