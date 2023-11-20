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

using DustInTheWind.DirectoryCompare.DataStructures;

namespace DustInTheWind.DirectoryCompare.Cli.Application.SnapshotArea.PresentSnapshot;

public class PresentSnapshotResponse
{
    public string PotName { get; set; }

    public Guid SnapshotId { get; set; }

    public string OriginalPath { get; set; }

    public DateTime SnapshotCreationTime { get; set; }

    public DirectoryDto RootDirectory { get; set; }

    public int TotalFileCount { get; set; }

    public int TotalDirectoryCount { get; set; }

    public DataSize DataSize { get; set; }

    public DataSize StorageSize { get; set; }
}