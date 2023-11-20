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

using DustInTheWind.DirectoryCompare.Cli.Application.SnapshotArea.PresentSnapshot;
using DustInTheWind.DirectoryCompare.DataStructures;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.SnapshotCommands.DisplaySnapshot;

public class SnapshotViewModel
{
    public string PotName { get; }

    public Guid SnapshotId { get; }

    public string OriginalPath { get; }

    public DateTime CreationTime { get; }

    public DirectoryDto RootDirectory { get; }

    public int TotalFileCount { get; }

    public int TotalDirectoryCount { get; }

    public DataSize DataSize { get; }

    public DataSize StorageSize { get; }

    public SnapshotViewModel(PresentSnapshotResponse response)
    {
        PotName = response.PotName;
        SnapshotId = response.SnapshotId;
        OriginalPath = response.OriginalPath;
        CreationTime = response.SnapshotCreationTime;
        RootDirectory = response.RootDirectory;
        TotalFileCount = response.TotalFileCount;
        TotalDirectoryCount = response.TotalDirectoryCount;
        DataSize = response.DataSize;
        StorageSize = response.StorageSize;
    }
}