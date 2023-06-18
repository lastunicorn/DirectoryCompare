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

using DustInTheWind.DirectoryCompare.Domain.Comparison;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace DustInTheWind.DirectoryCompare.Tests.Domain.Comparison.SnapshotComparerTests
{
    public class IdenticalSnapshotsWithTwoDifferentFilesTests
    {
        private readonly SnapshotComparer snapshotComparer;

        public IdenticalSnapshotsWithTwoDifferentFilesTests()
        {
            Snapshot snapshot1 = new();
            snapshot1.Files.AddRange(new[]
            {
                new HFile
                {
                    Name = "File1.txt",
                    Hash = new byte[] { 1, 2, 3 }
                },
                new HFile
                {
                    Name = "File2.txt", 
                    Hash = new byte[] { 10, 20, 30 }
                }
            });
            
            Snapshot snapshot2 = new();
            snapshot2.Files.AddRange(new[]
            {
                new HFile
                {
                    Name = "File1.txt",
                    Hash = new byte[] { 1, 2, 3 }
                },
                new HFile
                {
                    Name = "File2.txt", 
                    Hash = new byte[] { 10, 20, 30 }
                }
            });
            
            snapshotComparer = new SnapshotComparer(snapshot1, snapshot2);
        }

        [Fact]
        public void OnlyInSnapshot1_is_empty()
        {
            snapshotComparer.Compare();

            snapshotComparer.OnlyInSnapshot1.Should().BeEmpty();
        }

        [Fact]
        public void OnlyInSnapshot2_is_empty()
        {
            snapshotComparer.Compare();

            snapshotComparer.OnlyInSnapshot2.Should().BeEmpty();
        }

        [Fact]
        public void DifferentNames_is_empty()
        {
            snapshotComparer.Compare();

            snapshotComparer.DifferentNames.Should().BeEmpty();
        }

        [Fact]
        public void DifferentContent_is_empty()
        {
            snapshotComparer.Compare();

            snapshotComparer.DifferentContent.Should().BeEmpty();
        }
    }
}