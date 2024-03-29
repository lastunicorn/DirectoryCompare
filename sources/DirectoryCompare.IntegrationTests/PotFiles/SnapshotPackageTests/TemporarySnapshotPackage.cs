// Directory Compare
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
using DustInTheWind.DirectoryCompare.DataAccess.Transformations;
using DustInTheWind.DirectoryCompare.Domain.Entities;

namespace DustInTheWind.DirectoryCompare.IntegrationTests.PotFiles.SnapshotPackageTests;

internal class TemporarySnapshotPackage : IDisposable
{
    public string FilePath { get; }

    public TemporarySnapshotPackage(Snapshot snapshot)
    {
        FilePath = CreateSnapshotPackage(snapshot);
    }

    private static SnapshotFilePath CreateSnapshotPackage(Snapshot snapshot)
    {
        string temporaryPath = Path.GetTempPath();
        SnapshotFilePath snapshotFilePath = new(DateTime.UtcNow, temporaryPath);

        SnapshotPackage snapshotPackage = new(snapshotFilePath)
        {
            SnapshotContent = snapshot.ToJSnapshot()
        };
        snapshotPackage.Save();

        return snapshotFilePath;
    }

    public void Dispose()
    {
        File.Delete(FilePath);
    }
}