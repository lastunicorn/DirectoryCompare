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

using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.DataAccess.PotFiles.SnapshotFileModel;

public class JSnapshot
{
    [JsonProperty("serializer")]
    public SerializerInfo SerializerInfo { get; set; }

    [JsonProperty("id")]
    public Guid Id { get; set; }

    [JsonProperty("analysis-id")]
    public Guid AnalysisId { get; set; }

    [JsonProperty("original-path")]
    public string OriginalPath { get; set; }

    [JsonProperty("creation-time")]
    public DateTime CreationTime { get; set; }

    [JsonProperty("d")]
    public List<JDirectory> Directories { get; set; }

    [JsonProperty("f")]
    public List<JFile> Files { get; set; }
}