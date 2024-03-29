﻿// DirectoryCompare
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

using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.DataAccess.PotFiles.SnapshotFileModel;

public sealed class JSnapshotWriter : JDirectoryWriter
{
    public JSnapshotWriter(JsonTextWriter jsonTextWriter)
        : base(jsonTextWriter)
    {
    }

    public void WriteSerializerId(Guid id)
    {
        Writer.WritePropertyName("serializer-id");
        Writer.WriteValue(id);
    }

    public void WriteAnalysisId(Guid analysisId)
    {
        Writer.WritePropertyName("analysis-id");
        Writer.WriteValue(analysisId);
    }

    public void WriteOriginalPath(string originalPath)
    {
        Writer.WritePropertyName("original-path");
        Writer.WriteValue(originalPath);
    }

    public void WriteCreationTime(DateTime creationTime)
    {
        Writer.WritePropertyName("creation-time");
        Writer.WriteValue(creationTime);
    }
}