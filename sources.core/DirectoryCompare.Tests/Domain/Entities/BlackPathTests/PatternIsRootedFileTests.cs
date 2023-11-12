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

using DustInTheWind.DirectoryCompare.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace DustInTheWind.DirectoryCompare.Tests.Domain.Entities.BlackPathTests;

public class PatternIsRootedFileTests
{
    private readonly BlackPath blackPath;

    public PatternIsRootedFileTests()
    {
        blackPath = new BlackPath("/file-or-dir-1");
    }

    [Fact]
    public void HavingFileWithMatchingName_WhenChecked_ThenMatches()
    {
        // pattern: /file-or-dir-1
        // path:    /file-or-dir-1 (file)

        HFile hFile = new()
        {
            Name = "file-or-dir-1"
        };

        bool actual = blackPath.Matches(hFile);

        actual.Should().BeTrue();
    }

    [Fact]
    public void HavingDirectoryWithMatchingName_WhenChecked_ThenMatches()
    {
        // pattern: /file-or-dir-1
        // path:    /file-or-dir-1 (dir)

        HDirectory hDirectory = new()
        {
            Name = "file-or-dir-1"
        };

        bool actual = blackPath.Matches(hDirectory);

        actual.Should().BeTrue();
    }

    [Fact]
    public void HavingFileWithMatchingNamePlacedInPath_WhenChecked_ThenDoesNotMatch()
    {
        // pattern: /file-or-dir-1
        // path:    /dir-2/file-or-dir-1 (file)

        HFile hFile = new()
        {
            Name = "file-or-dir-1",
            Parent = new HDirectory
            {
                Name = "dir-2"
            }
        };

        bool actual = blackPath.Matches(hFile);

        actual.Should().BeFalse();
    }

    [Fact]
    public void HavingDirectoryWithMatchingNamePlacedInPath_WhenChecked_ThenDoesNotMatch()
    {
        // pattern: /file-or-dir-1
        // path:    /dir-2/file-or-dir-1 (dir)

        HDirectory hDirectory = new()
        {
            Name = "file-or-dir-1",
            Parent = new HDirectory
            {
                Name = "dir-2"
            }
        };

        bool actual = blackPath.Matches(hDirectory);

        actual.Should().BeFalse();
    }
}