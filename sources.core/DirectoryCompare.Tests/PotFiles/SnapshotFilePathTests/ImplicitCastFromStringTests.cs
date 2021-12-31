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

using System;
using DustInTheWind.DirectoryCompare.JFiles;
using NUnit.Framework;

namespace DustInTheWind.DirectoryCompare.Tests.PotFiles.SnapshotFilePathTests
{
    [TestFixture]
    public class ImplicitCastFromStringTests
    {
        [Test]
        public void HavingFilePathString_WhenImplicitlyCastFromString_ThenCreationTimeIsParsedCorrectly()
        {
            // arrange
            const string pathAsString = "/this/is/some/path/2021 12 31 143918.json";

            // act
            SnapshotFilePath actual = pathAsString;

            // assert
            DateTime expected = new DateTime(2021, 12, 31, 14, 39, 18);
            Assert.That(actual.CreationTime, Is.EqualTo(expected));
        }
    }
}