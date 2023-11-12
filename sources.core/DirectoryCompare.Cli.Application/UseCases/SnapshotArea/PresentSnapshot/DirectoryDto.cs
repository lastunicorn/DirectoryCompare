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

using DustInTheWind.DirectoryCompare.Domain.Entities;

namespace DustInTheWind.DirectoryCompare.Cli.Application.UseCases.SnapshotArea.PresentSnapshot;

public class DirectoryDto
{
    public string Name { get; }

    public List<DirectoryDto> Directories { get; }

    public List<FileDto> Files { get; }
    
    public DirectoryDto(HDirectory hDirectory)
    {
        if (hDirectory == null)
            return;

        Name = hDirectory.Name;
        Files = hDirectory.Files
            .Select(x => new FileDto(x))
            .ToList();
        Directories = hDirectory.Directories
            .Select(x => new DirectoryDto(x))
            .ToList();
    }
}