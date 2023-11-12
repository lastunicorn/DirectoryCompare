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

using DustInTheWind.DirectoryCompare.JFiles;
using FluentAssertions;
using Xunit;

namespace DustInTheWind.DirectoryCompare.Tests.Adapters.PotFiles.SnapshotFilePathTests;

public class ToStringTests
{
    [Fact]
    public void HavingSnapshotFilePathInstance_WhenToStringCalled_ThenStringContainsFullPath()
    {
        // arrange
        const string pathAsString = "/this/is/some/path/2021 12 31 143918.json";
        SnapshotFilePath snapshotFilePath = new(pathAsString);

        // act
        string actual = snapshotFilePath.ToString();

        // assert
        actual.Should().Be(pathAsString);
    }
}