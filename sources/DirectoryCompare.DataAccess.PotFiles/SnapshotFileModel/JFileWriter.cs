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

using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.DataAccess.PotFiles.SnapshotFileModel;

public class JFileWriter
{
    protected JsonTextWriter Writer { get; }

    public JFileWriter(JsonTextWriter jsonTextWriter)
    {
        Writer = jsonTextWriter ?? throw new ArgumentNullException(nameof(jsonTextWriter));
    }

    public void WriteStart()
    {
        Writer.WriteStartObject();
    }

    public void WriteName(string fileName)
    {
        Writer.WritePropertyName("n");
        Writer.WriteValue(fileName);
    }

    public void WriteSize(ulong fileSize)
    {
        Writer.WritePropertyName("s");
        Writer.WriteValue(fileSize);
    }

    public void WriteLastModifiedTime(DateTime lastModifiedTime)
    {
        Writer.WritePropertyName("m");
        Writer.WriteValue(lastModifiedTime);
    }

    public void WriteHash(byte[] hash)
    {
        Writer.WritePropertyName("h");
        Writer.WriteValue(hash);
    }

    public void WriteEnd()
    {
        Writer.WriteEndObject();
    }
}