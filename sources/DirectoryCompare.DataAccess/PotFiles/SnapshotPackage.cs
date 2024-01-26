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

using DustInTheWind.DirectoryCompare.DataAccess.FileDatabase;
using DustInTheWind.DirectoryCompare.DataAccess.PotFiles.SnapshotFileModel;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.DataAccess.PotFiles;

public class SnapshotPackage : PackageFile
{
    private readonly SnapshotFilePath snapshotFilePath;
    private readonly PackageDocument snapshotPackageDocument;

    public SnapshotPackage(string filePath)
        : base(filePath)
    {
        snapshotFilePath = filePath;

        snapshotPackageDocument = new PackageDocument
        {
            Name = "snapshot.json",
            Type = typeof(JSnapshot)
        };

        Documents.Add(snapshotPackageDocument);
    }

    public DateTime? CreationTime => snapshotFilePath.CreationTime;

    public JSnapshot SnapshotContent
    {
        get => snapshotPackageDocument.Content as JSnapshot;
        set => snapshotPackageDocument.Content = value;
    }

    public JSnapshotWriter OpenSnapshotWriter()
    {
        JsonTextWriter jsonTextWriter = OpenWriter("snapshot.json");
        return new JSnapshotWriter(jsonTextWriter);
    }
}