// DirectoryCompare
// Copyright (C) 2017-2020 Dust in the Wind
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

using DustInTheWind.DirectoryCompare.DataAccess;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Domain.Utils;
using FluentAssertions;
using Xunit;

namespace DustInTheWind.DirectoryCompare.Tests.Adapters.DataAccess.PotImportExportTests;

public class WriteSnapshotTests
{
    [Fact]
    public void SerializeEmptySnapshot()
    {
        Snapshot snapshot = new()
        {
            CreationTime = new DateTime(2019, 05, 08, 19, 17, 00, DateTimeKind.Utc),
            Name = "Snapshot 1",
            OriginalPath = @"c:\aaa"
        };

        string expected = EmbeddedResources.GetContent("Data-EmptySnapshot.json");
        PerformTest(snapshot, expected);
    }

    [Fact]
    public void SerializeSnapshotWithOneDirectory()
    {
        Snapshot snapshot = new()
        {
            CreationTime = new DateTime(2019, 05, 08, 19, 17, 00, DateTimeKind.Utc),
            Name = "Snapshot 1",
            OriginalPath = @"c:\aaa"
        };
        snapshot.Directories.Add(new HDirectory
        {
            Name = "directory-name"
        });

        string expected = EmbeddedResources.GetContent("Data-SnapshotWithOneDirectory.json");
        PerformTest(snapshot, expected);
    }

    [Fact]
    public void SerializeSnapshotWithOneFile()
    {
        Snapshot snapshot = new()
        {
            CreationTime = new DateTime(2019, 05, 08, 19, 17, 00, DateTimeKind.Utc),
            Name = "Snapshot 1",
            OriginalPath = @"c:\aaa"
        };
        snapshot.Files.Add(new HFile
        {
            Name = "file.extension",
            Size = DataSize.FromKilobytes(42),
            LastModifiedTime = new DateTime(2011, 05, 13, 12, 56, 20),
            Hash = new byte[] { 0, 1, 2 }
        });

        string expected = EmbeddedResources.GetContent("Data-SnapshotWithOneFile.json");
        PerformTest(snapshot, expected);
    }

    private static void PerformTest(Snapshot snapshot, string expected)
    {
        using MemoryStream memoryStream = new();
        using StreamWriter streamWriter = new(memoryStream);

        PotImportExport.WriteSnapshot(snapshot, streamWriter);

        streamWriter.Flush();
        memoryStream.Flush();

        memoryStream.Position = 0;

        using StreamReader streamReader = new(memoryStream);
        string json = streamReader.ReadToEnd();

        json.Should().Be(expected);
    }
}