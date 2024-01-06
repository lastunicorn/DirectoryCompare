// DirectoryCompare
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

using DustInTheWind.DirectoryCompare.DataAccess.PotFiles;
using FluentAssertions;
using Xunit;

namespace DustInTheWind.DirectoryCompare.Tests.Adapters.PotFiles.SnapshotFilePathTests;

public class ImplicitCastFromStringTests
{
    [Fact]
    public void HavingFilePathString_WhenImplicitlyCastFromString_ThenCreationTimeIsParsedCorrectly()
    {
        // arrange
        const string pathAsString = "/this/is/some/path/2021 12 31 143918.json";

        // act
        SnapshotFilePath actual = pathAsString;

        // assert
        DateTime expected = new(2021, 12, 31, 14, 39, 18);
        actual.CreationTime.Should().Be(expected);
    }
}