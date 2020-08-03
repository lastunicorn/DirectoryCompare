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

using System;
using DustInTheWind.DirectoryCompare.Domain;
using NUnit.Framework;

namespace DustInTheWind.DirectoryCompare.Tests.Domain
{
    [TestFixture]
    public class SnapshotLocationTests
    {
        [Test]
        public void OnlyPotNameIsSpecified()
        {
            SnapshotLocation snapshotLocation = new SnapshotLocation("pot1");

            Assert.That(snapshotLocation.PotName, Is.EqualTo("pot1"), () => "Pot name is not correct.");
            Assert.That(snapshotLocation.SnapshotDate, Is.Null, () => "Snapshot date is not correct.");
            Assert.That(snapshotLocation.SnapshotIndex, Is.Null, () => "Snapshot index is not correct.");
            Assert.That(snapshotLocation.InternalPath, Is.Null, () => "Internal path is not correct.");
        }

        [Test]
        public void PotNameAndSnapshotIndex5()
        {
            SnapshotLocation snapshotLocation = new SnapshotLocation("pot1~5");

            Assert.That(snapshotLocation.PotName, Is.EqualTo("pot1"), () => "Pot name is not correct.");
            Assert.That(snapshotLocation.SnapshotDate, Is.Null, () => "Snapshot date is not correct.");
            Assert.That(snapshotLocation.SnapshotIndex, Is.EqualTo(5), () => "Snapshot index is not correct.");
            Assert.That(snapshotLocation.InternalPath, Is.Null, () => "Internal path is not correct.");
        }

        [Test]
        public void PotNameAndSnapshotIndex5AndPath()
        {
            SnapshotLocation snapshotLocation = new SnapshotLocation("pot1~5>asd/qwe");

            Assert.That(snapshotLocation.PotName, Is.EqualTo("pot1"), () => "Pot name is not correct.");
            Assert.That(snapshotLocation.SnapshotDate, Is.Null, () => "Snapshot date is not correct.");
            Assert.That(snapshotLocation.SnapshotIndex, Is.EqualTo(5), () => "Snapshot index is not correct.");
            Assert.That(snapshotLocation.InternalPath, Is.EqualTo("asd/qwe"), () => "Internal path is not correct.");
        }

        [Test]
        public void PotNameAndSnapshotDate()
        {
            SnapshotLocation snapshotLocation = new SnapshotLocation("pot1~2020-05-12 12:34:00");

            Assert.That(snapshotLocation.PotName, Is.EqualTo("pot1"), () => "Pot name is not correct.");
            Assert.That(snapshotLocation.SnapshotDate, Is.EqualTo(new DateTime(2020, 05, 12, 12, 34, 00)), () => "Snapshot date is not correct.");
            Assert.That(snapshotLocation.SnapshotIndex, Is.Null, () => "Snapshot index is not correct.");
            Assert.That(snapshotLocation.InternalPath, Is.Null, () => "Internal path is not correct.");
        }

        [Test]
        public void PotNameAndSnapshotDateAndPath()
        {
            SnapshotLocation snapshotLocation = new SnapshotLocation("pot1~2020-05-12 12:34:00>asd/qwe");

            Assert.That(snapshotLocation.PotName, Is.EqualTo("pot1"), () => "Pot name is not correct.");
            Assert.That(snapshotLocation.SnapshotDate, Is.EqualTo(new DateTime(2020, 05, 12, 12, 34, 00)), () => "Snapshot date is not correct.");
            Assert.That(snapshotLocation.SnapshotIndex, Is.Null, () => "Snapshot index is not correct.");
            Assert.That(snapshotLocation.InternalPath, Is.EqualTo("asd/qwe"), () => "Internal path is not correct.");
        }
    }
}