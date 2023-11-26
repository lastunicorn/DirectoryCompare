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

using DustInTheWind.DirectoryCompare.Cli.Application.PotArea.PresentPots;
using DustInTheWind.DirectoryCompare.DataStructures;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.PotCommands.DisplayPots;

public class PotViewModel
{
    public string Name { get; }

    public Guid Guid { get; }

    public string Path { get; }
    
    public DataSize Size { get; }

    public PotViewModel(PotDto potDto)
    {
        Name = potDto.Name;
        Guid = potDto.Guid;
        Path = potDto.Path;
        Size = potDto.Size;
    }
}