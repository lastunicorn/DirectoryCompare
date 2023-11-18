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

using DustInTheWind.DirectoryCompare.Domain.Comparison;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace DustInTheWind.DirectoryCompare.Tests.Domain.Comparison.SnapshotComparisonTests;

public class CompareDirectoryWithOneFileTests
{
    #region OnlyInSnapshot1

    [Fact]
    public void OnlyInSnapshot1_is_empty_if_both_snapshots_contain_one_identical_file_in_same_dir()
    {
        Snapshot snapshot1 = new();
        HDirectory hDirectory1 = new("Dir1");
        hDirectory1.Files.AddRange(new[]
        {
            new HFile { Name = "File1", Hash = new byte[] { 0x01, 0x02, 0x03 } }
        });
        snapshot1.Directories.AddRange(new[] { hDirectory1 });

        Snapshot snapshot2 = new();
        HDirectory hDirectory2 = new("Dir1");
        hDirectory2.Files.AddRange(new[]
        {
            new HFile { Name = "File1", Hash = new byte[] { 0x01, 0x02, 0x03 } }
        });
        snapshot2.Directories.AddRange(new[] { hDirectory2 });

        SnapshotComparison comparison = new(snapshot1, snapshot2);
        comparison.Compare();

        comparison.OnlyInSnapshot1.Should().BeEmpty();
    }

    [Fact]
    public void OnlyInSnapshot1_contains_the_name_of_the_file_if_only_snapshot1_has_one_file_in_dir()
    {
        Snapshot snapshot1 = new();
        HDirectory hDirectory1 = new("Dir1");
        hDirectory1.Files.AddRange(new[]
        {
            new HFile { Name = "File1", Hash = new byte[] { 0x01, 0x02, 0x03 } }
        });
        snapshot1.Directories.AddRange(new[] { hDirectory1 });

        Snapshot snapshot2 = new();
        HDirectory hDirectory2 = new("Dir1");
        snapshot2.Directories.AddRange(new[] { hDirectory2 });

        SnapshotComparison comparison = new(snapshot1, snapshot2);
        comparison.Compare();

        comparison.OnlyInSnapshot1.Should().Equal(new[] { "/Dir1/File1" });
    }

    [Fact]
    public void OnlyInSnapshot1_is_empty_if_only_snapshot2_has_one_file_in_dir()
    {
        Snapshot snapshot1 = new();
        HDirectory hDirectory1 = new("Dir1");
        snapshot1.Directories.AddRange(new[] { hDirectory1 });

        Snapshot snapshot2 = new();
        HDirectory hDirectory2 = new("Dir1");
        hDirectory2.Files.AddRange(new[]
        {
            new HFile { Name = "File1", Hash = new byte[] { 0x01, 0x02, 0x03 } }
        });
        snapshot2.Directories.AddRange(new[] { hDirectory2 });

        SnapshotComparison comparison = new(snapshot1, snapshot2);
        comparison.Compare();

        comparison.OnlyInSnapshot1.Should().BeEmpty();
    }

    #endregion

    #region OnlyInSnapshot2

    [Fact]
    public void OnlyInSnapshot2_is_empty_if_both_snapshots_contain_one_identical_file_in_same_dir()
    {
        Snapshot snapshot1 = new();
        HDirectory hDirectory1 = new("Dir1");
        hDirectory1.Files.AddRange(new[]
        {
            new HFile { Name = "File1", Hash = new byte[] { 0x01, 0x02, 0x03 } }
        });
        snapshot1.Directories.AddRange(new[] { hDirectory1 });

        Snapshot snapshot2 = new();
        HDirectory hDirectory2 = new("Dir1");
        hDirectory2.Files.AddRange(new[]
        {
            new HFile { Name = "File1", Hash = new byte[] { 0x01, 0x02, 0x03 } }
        });
        snapshot2.Directories.AddRange(new[] { hDirectory2 });

        SnapshotComparison comparison = new(snapshot1, snapshot2);
        comparison.Compare();

        comparison.OnlyInSnapshot2.Should().BeEmpty();
    }

    [Fact]
    public void OnlyInSnapshot2_contains_the_name_of_the_file_if_only_snapshot2_has_one_file_in_dir()
    {
        Snapshot snapshot1 = new();
        HDirectory hDirectory1 = new("Dir1");
        snapshot1.Directories.AddRange(new[] { hDirectory1 });

        Snapshot snapshot2 = new();
        HDirectory hDirectory2 = new("Dir1");
        hDirectory2.Files.AddRange(new[]
        {
            new HFile { Name = "File1", Hash = new byte[] { 0x01, 0x02, 0x03 } }
        });
        snapshot2.Directories.AddRange(new[] { hDirectory2 });

        SnapshotComparison comparison = new(snapshot1, snapshot2);
        comparison.Compare();

        comparison.OnlyInSnapshot2.Should().Equal(new[] { "/Dir1/File1" });
    }

    [Fact]
    public void OnlyInSnapshot2_is_empty_if_only_snapshot1_has_one_file_in_dir()
    {
        Snapshot snapshot1 = new();
        HDirectory hDirectory1 = new("Dir1");
        hDirectory1.Files.AddRange(new[]
        {
            new HFile { Name = "File1", Hash = new byte[] { 0x01, 0x02, 0x03 } }
        });
        snapshot1.Directories.AddRange(new[] { hDirectory1 });

        Snapshot snapshot2 = new();
        HDirectory hDirectory2 = new("Dir1");
        snapshot2.Directories.AddRange(new[] { hDirectory2 });

        SnapshotComparison comparison = new(snapshot1, snapshot2);
        comparison.Compare();

        comparison.OnlyInSnapshot2.Should().BeEmpty();
    }

    #endregion
}