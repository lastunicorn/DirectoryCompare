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

using DustInTheWind.DirectoryCompare.DataStructures;
using DustInTheWind.DirectoryCompare.Domain.PotModel;

namespace DustInTheWind.DirectoryCompare.Cli.Application.PotArea.PresentPots;

public class PotDto
{
    public string Name { get; }

    public Guid Guid { get; }

    public string Path { get; }

    public DataSize Size { get; set; }

    public bool HasPathFilters { get; set; }

    public PotDto(Pot pot)
    {
        if (pot == null)
            return;

        Name = pot.Name;
        Guid = pot.Guid;
        Path = pot.Path;
        HasPathFilters = pot.IncludedPaths?.Count > 0;
    }
}