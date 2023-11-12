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

public class PatternIsDirectoryTests
{
    private readonly BlackPath blackPath;

    public PatternIsDirectoryTests()
    {
        blackPath = new BlackPath("item-1/");
    }

    [Fact]
    public void HavingFileWithMatchingName_WhenChecked_ThenDoesNotMatch()
    {
        // pattern: item-1/
        // path:    /item-1 (file)

        HFile hFile = new()
        {
            Name = "item-1"
        };

        bool actual = blackPath.Matches(hFile);

        actual.Should().BeFalse();
    }

    [Fact]
    public void HavingDirectoryWithMatchingName_WhenChecked_ThenMatches()
    {
        // pattern: item-1/
        // path:    /item-1 (dir)

        HDirectory hDirectory = new()
        {
            Name = "item-1"
        };

        bool actual = blackPath.Matches(hDirectory);

        actual.Should().BeTrue();
    }

    [Fact]
    public void HavingFileWithMatchingNamePlacedInPath_WhenChecked_ThenDoesNotMatch()
    {
        // pattern: item-1/
        // path:    /dir-2/item-1 (file)

        HFile hFile = new()
        {
            Name = "item-1",
            Parent = new HDirectory
            {
                Name = "dir-2"
            }
        };

        bool actual = blackPath.Matches(hFile);

        actual.Should().BeFalse();
    }

    [Fact]
    public void HavingDirectoryWithMatchingNamePlacedInPath_WhenChecked_ThenMatches()
    {
        // pattern: item-1/
        // path:    /dir-2/item-1 (dir)

        HDirectory hDirectory = new()
        {
            Name = "item-1",
            Parent = new HDirectory
            {
                Name = "dir-2"
            }
        };

        bool actual = blackPath.Matches(hDirectory);

        actual.Should().BeTrue();
    }

    [Fact]
    public void HavingFilePlacedInDirectoryWithMatchingname_WhenChecked_ThenMatches()
    {
        // pattern: item-1/
        // path:    /item-1/file-1 (file)

        HFile hFile = new()
        {
            Name = "file-1",
            Parent = new HDirectory
            {
                Name = "item-1"
            }
        };

        bool actual = blackPath.Matches(hFile);

        actual.Should().BeTrue();
    }
}