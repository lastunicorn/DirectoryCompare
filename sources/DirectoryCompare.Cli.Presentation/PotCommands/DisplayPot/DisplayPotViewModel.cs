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

using DustInTheWind.DirectoryCompare.Cli.Application.PotArea.PresentPot;
using DustInTheWind.DirectoryCompare.DataStructures;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.PotCommands.DisplayPot;

public class DisplayPotViewModel
{
    public bool Exists { get; }

    public PotViewModel PotViewModel { get; }

    public List<SnapshotViewModel> Snapshots { get; }

    public List<SnapshotPath> IncludedPaths { get; set; }

    public DisplayPotViewModel(PotDto pot)
    {
        if (pot == null)
            return;

        Exists = true;
        PotViewModel = new PotViewModel
        {
            Name = pot.Name,
            Guid = pot.Guid,
            Path = pot.Path,
            IncludedPaths = pot.IncludedPaths,
            Description = pot.Description,
            Size = pot.Size
        };

        Snapshots = pot.Snapshots?
            .Select(x => new SnapshotViewModel(x))
            .ToList();
    }
}