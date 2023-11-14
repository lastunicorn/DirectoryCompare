// VeloCity
// Copyright (C) 2022-2023 Dust in the Wind
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

using DustInTheWind.DirectoryCompare.Cli.Application.PotArea.PresentPot;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.PotCommands.DisplayPot;

public class PotViewModel
{
    public string Name { get; }

    public Guid Guid { get; }

    public string Path { get; }

    public string Description { get; }

    public List<SnapshotViewModel> Snapshots { get; }

    public PotViewModel(PresentPotResponse response)
    {
        Name = response.PotName;
        Guid = response.PotGuid;
        Path = response.PotPath;
        Description = response.PotDescription;
        Snapshots = response.Snapshots
            .Select(x => new SnapshotViewModel(x))
            .ToList();
    }
}