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

using DustInTheWind.DirectoryCompare.DataAccess.PotFiles;
using Xunit;

namespace DustInTheWind.DirectoryCompare.Tests.Adapters.PotFiles.SnapshotFilePathTests;

public class ConstructorTests
{
    [Fact]
    public void HavingNullDirectoryPath_WhenInstanceIsCreated_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            new SnapshotFilePath(null);
        });
    }

    [Fact]
    public void HavingAbsolutePathEndingWithSlash_WhenInstanceIsCreated_ThrowsFileNameNotSpecifiedException()
    {
        const string snapshotFilePathString = "/this/is/some/path/";

        Assert.Throws<FileNameNotSpecifiedException>(() =>
        {
            new SnapshotFilePath(snapshotFilePathString);
        });
    }

    [Fact]
    public void HavingAbsoluteFileNamePathWithoutExtension_WhenInstanceIsCreated_ThrowsInvalidSnapshotFileNameException()
    {
        const string snapshotFilePathString = "/this/is/some/path/2021 12 31 143918";

        Assert.Throws<InvalidSnapshotFileNameException>(() =>
        {
            new SnapshotFilePath(snapshotFilePathString);
        });
    }

    [Fact]
    public void HavingAbsoluteFileNamePathWithWrongExtension_WhenInstanceIsCreated_ThrowsInvalidSnapshotFileNameException()
    {
        const string snapshotFilePathString = "/this/is/some/path/2021 12 31 143918.txt";

        Assert.Throws<InvalidSnapshotFileNameException>(() =>
        {
            new SnapshotFilePath(snapshotFilePathString);
        });
    }

    [Fact]
    public void HavingAbsoluteFileNamePathWithWrongName_WhenInstanceIsCreated_ThrowsInvalidSnapshotFileNameException()
    {
        const string snapshotFilePathString = "/this/is/some/path/file.json";

        Assert.Throws<InvalidSnapshotFileNameException>(() =>
        {
            new SnapshotFilePath(snapshotFilePathString);
        });
    }

    [Fact]
    public void HavingAbsoluteFileNamePath_WhenInstanceIsCreated_ThenSuccess()
    {
        const string snapshotFilePathString = "/this/is/some/path/2021 12 31 143918.json";

        new SnapshotFilePath(snapshotFilePathString);
    }
}