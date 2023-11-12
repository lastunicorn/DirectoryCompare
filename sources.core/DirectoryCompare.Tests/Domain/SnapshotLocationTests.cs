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
using FluentAssertions;
using Xunit;

namespace DustInTheWind.DirectoryCompare.Tests.Domain;

public class SnapshotLocationTests
{
    [Fact]
    public void OnlyPotNameIsSpecified()
    {
        SnapshotLocation snapshotLocation = new("pot1");

        snapshotLocation.PotName.Should().Be("pot1", "Pot name is not correct.");
        snapshotLocation.SnapshotDate.Should().BeNull("Snapshot date is not correct.");
        snapshotLocation.SnapshotIndex.Should().BeNull("Snapshot index is not correct.");
        snapshotLocation.InternalPath.Should().BeNull("Internal path is not correct.");
    }

    [Fact]
    public void PotNameAndSnapshotIndex5()
    {
        SnapshotLocation snapshotLocation = new("pot1~5");

        snapshotLocation.PotName.Should().Be("pot1", "Pot name is not correct.");
        snapshotLocation.SnapshotDate.Should().BeNull("Snapshot date is not correct.");
        snapshotLocation.SnapshotIndex.Should().Be(5, "Snapshot index is not correct.");
        snapshotLocation.InternalPath.Should().BeNull("Internal path is not correct.");
    }

    [Fact]
    public void PotNameAndSnapshotIndex5AndPath()
    {
        SnapshotLocation snapshotLocation = new("pot1~5>asd/qwe");

        snapshotLocation.PotName.Should().Be("pot1", "Pot name is not correct.");
        snapshotLocation.SnapshotDate.Should().BeNull("Snapshot date is not correct.");
        snapshotLocation.SnapshotIndex.Should().Be(5, "Snapshot index is not correct.");
        snapshotLocation.InternalPath.Should().Be("asd/qwe", "Internal path is not correct.");
    }

    [Fact]
    public void PotNameAndSnapshotDate()
    {
        SnapshotLocation snapshotLocation = new("pot1~2020-05-12 12:34:00");

        snapshotLocation.PotName.Should().Be("pot1", "Pot name is not correct.");
        snapshotLocation.SnapshotDate.Should().Be(new DateTime(2020, 05, 12, 12, 34, 00), "Snapshot date is not correct.");
        snapshotLocation.SnapshotIndex.Should().BeNull("Snapshot index is not correct.");
        snapshotLocation.InternalPath.Should().BeNull("Internal path is not correct.");
    }

    [Fact]
    public void PotNameAndSnapshotDateAndPath()
    {
        SnapshotLocation snapshotLocation = new("pot1~2020-05-12 12:34:00>asd/qwe");

        snapshotLocation.PotName.Should().Be("pot1", "Pot name is not correct.");
        snapshotLocation.SnapshotDate.Should().Be(new DateTime(2020, 05, 12, 12, 34, 00), "Snapshot date is not correct.");
        snapshotLocation.SnapshotIndex.Should().BeNull("Snapshot index is not correct.");
        snapshotLocation.InternalPath.Should().Be("asd/qwe", "Internal path is not correct.");
    }
}