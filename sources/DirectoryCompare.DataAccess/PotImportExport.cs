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

using DustInTheWind.DirectoryCompare.DataAccess.PotFiles.SnapshotFileModel;
using DustInTheWind.DirectoryCompare.DataAccess.Transformations;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Ports.DataAccess.ImportExport;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.DataAccess;

public class PotImportExport : IPotImportExport
{
    public Snapshot ReadSnapshot(string filePath)
    {
        using StreamReader streamReader = File.OpenText(filePath);
        using JsonTextReader jsonTextReader = new(streamReader);

        JsonSerializer serializer = new();
        JSnapshot jSnapshot = (JSnapshot)serializer.Deserialize(jsonTextReader, typeof(JSnapshot));

        return jSnapshot.ToSnapshot();
    }

    public void WriteSnapshot(Snapshot snapshot, string filePath)
    {
        using StreamWriter streamWriter = new(filePath);
        WriteSnapshot(snapshot, streamWriter);
    }

    internal static void WriteSnapshot(Snapshot snapshot, StreamWriter streamWriter)
    {
        JsonTextWriter jsonTextWriter = new(streamWriter);
        jsonTextWriter.Formatting = Formatting.Indented;

        JSnapshotWriter jSnapshotWriter = new(jsonTextWriter);
        JsonSnapshotWriter jsonSnapshotWriter = new(jSnapshotWriter);

        jsonSnapshotWriter.Write(snapshot);
    }
}