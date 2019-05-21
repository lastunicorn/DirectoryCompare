// DirectoryCompare
// Copyright (C) 2017-2019 Dust in the Wind
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

using DustInTheWind.DirectoryCompare.Comparison;
using DustInTheWind.DirectoryCompare.Entities;
using NUnit.Framework;

namespace DustInTheWind.DirectoryCompare.Tests.SnapshotComparerTests
{
    [TestFixture]
    public class CompareOneFileTests
    {
        #region OnlyInSnapshot1

        [Test]
        public void OnlyInSnapshot1_is_empty_if_both_snapshots_contain_one_identical_file()
        {
            Snapshot snapshot1 = new Snapshot();
            snapshot1.Files.AddRange(new[]
            {
                new HFile { Name = "File1", Hash = new byte[] { 0x01, 0x02, 0x03 } }
            });

            Snapshot snapshot2 = new Snapshot();
            snapshot2.Files.AddRange(new[]
            {
                new HFile { Name = "File1", Hash = new byte[] { 0x01, 0x02, 0x03 } }
            });

            SnapshotComparer comparer = new SnapshotComparer(snapshot1, snapshot2);
            comparer.Compare();

            Assert.That(comparer.OnlyInSnapshot1, Is.EqualTo(new string[0]));
        }

        [Test]
        public void OnlyInSnapshot1_contains_the_name_of_the_file_if_only_snapshot1_has_one_file()
        {
            Snapshot snapshot1 = new Snapshot();
            snapshot1.Files.AddRange(new[]
            {
                new HFile { Name = "File1", Hash = new byte[] { 0x01, 0x02, 0x03 } }
            });

            Snapshot snapshot2 = new Snapshot();

            SnapshotComparer comparer = new SnapshotComparer(snapshot1, snapshot2);
            comparer.Compare();

            Assert.That(comparer.OnlyInSnapshot1, Is.EqualTo(new[] { "/File1" }));
        }

        [Test]
        public void OnlyInSnapshot1_is_empty_if_only_snapshot2_has_one_file()
        {
            Snapshot snapshot1 = new Snapshot();

            Snapshot snapshot2 = new Snapshot();
            snapshot2.Files.AddRange(new[]
            {
                new HFile { Name = "File1", Hash = new byte[] { 0x01, 0x02, 0x03 } }
            });

            SnapshotComparer comparer = new SnapshotComparer(snapshot1, snapshot2);
            comparer.Compare();

            Assert.That(comparer.OnlyInSnapshot1, Is.EqualTo(new string[0]));
        }

        #endregion

        #region OnlyInSnapshot2

        [Test]
        public void OnlyInSnapshot2_is_empty_if_both_snapshots_contain_one_identical_file()
        {
            Snapshot snapshot1 = new Snapshot();
            snapshot1.Files.AddRange(new[]
            {
                new HFile { Name = "File1", Hash = new byte[] { 0x01, 0x02, 0x03 } }
            });

            Snapshot snapshot2 = new Snapshot();
            snapshot2.Files.AddRange(new[]
            {
                new HFile { Name = "File1", Hash = new byte[] { 0x01, 0x02, 0x03 } }
            });

            SnapshotComparer comparer = new SnapshotComparer(snapshot1, snapshot2);
            comparer.Compare();

            Assert.That(comparer.OnlyInSnapshot2, Is.EqualTo(new string[0]));
        }

        [Test]
        public void OnlyInSnapshot2_contains_the_name_of_the_file_if_only_snapshot2_has_one_file()
        {
            Snapshot snapshot1 = new Snapshot();

            Snapshot snapshot2 = new Snapshot();
            snapshot2.Files.AddRange(new[]
            {
                new HFile { Name = "File1", Hash = new byte[] { 0x01, 0x02, 0x03 } }
            });

            SnapshotComparer comparer = new SnapshotComparer(snapshot1, snapshot2);
            comparer.Compare();

            Assert.That(comparer.OnlyInSnapshot2, Is.EqualTo(new[] { "/File1" }));
        }

        [Test]
        public void OnlyInSnapshot2_is_empty_if_only_snapshot1_has_one_file()
        {
            Snapshot snapshot1 = new Snapshot();
            snapshot1.Files.AddRange(new[]
            {
                new HFile { Name = "File1", Hash = new byte[] { 0x01, 0x02, 0x03 } }
            });

            Snapshot snapshot2 = new Snapshot();

            SnapshotComparer comparer = new SnapshotComparer(snapshot1, snapshot2);
            comparer.Compare();

            Assert.That(comparer.OnlyInSnapshot2, Is.EqualTo(new string[0]));
        }

        #endregion
    }
}