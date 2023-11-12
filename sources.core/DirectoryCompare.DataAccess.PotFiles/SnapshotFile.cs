﻿// DirectoryCompare
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

using DustInTheWind.DirectoryCompare.DataAccess.PotFiles.SnapshotFileModel;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.DataAccess.PotFiles;

public class SnapshotFile : JsonFileBase<JSnapshot>
{
    private readonly SnapshotFilePath snapshotFilePath;

    public DateTime? CreationTime => snapshotFilePath.CreationTime;

    public SnapshotFile(SnapshotFilePath snapshotFilePath)
        : base(snapshotFilePath)
    {
        this.snapshotFilePath = snapshotFilePath ?? throw new ArgumentNullException(nameof(snapshotFilePath));
    }

    public JSnapshotReader OpenSnapshotReader()
    {
        JsonTextReader jsonTextReader = OpenReader();
        return new JSnapshotReader(jsonTextReader);
    }

    public JSnapshotWriter OpenSnapshotWriter()
    {
        JsonTextWriter jsonTextWriter = OpenWriter();
        return new JSnapshotWriter(jsonTextWriter);
    }
}