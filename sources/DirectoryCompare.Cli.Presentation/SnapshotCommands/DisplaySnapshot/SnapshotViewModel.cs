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

using DustInTheWind.DirectoryCompare.Cli.Application.SnapshotArea.PresentSnapshot;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.SnapshotCommands.DisplaySnapshot;

public class SnapshotViewModel
{
    public string PotName { get; set; }

    public Guid SnapshotId { get; set; }

    public string OriginalPath { get; set; }

    public DateTime CreationTime { get; set; }

    public DirectoryDto RootDirectory { get; set; }

    public int TotalFileCount { get; set; }

    public int TotalDirectoryCount { get; set; }

    public SnapshotViewModel(PresentSnapshotResponse response)
    {
        PotName = response.PotName;
        SnapshotId = response.SnapshotId;
        OriginalPath = response.OriginalPath;
        CreationTime = response.SnapshotCreationTime;
        RootDirectory = response.RootDirectory;
        TotalFileCount = response.TotalFileCount;
        TotalDirectoryCount = response.TotalDirectoryCount;
    }
}