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

using DustInTheWind.DirectoryCompare.FileSystemAccess;
using DustInTheWind.DirectoryCompare.IntegrationTests.Utils;
using DustInTheWind.DirectoryCompare.Ports.FileSystemAccess;
using Xunit;

namespace DustInTheWind.DirectoryCompare.IntegrationTests.FileSystemAccess.DiskCrawlerTests;

[Collection("CrawlerTests")]
public class Crawl_SecondLevelChildrenTests : IDisposable
{
    private readonly PlaygroundDirectory playgroundDirectory = new();

    [Fact]
    public void HavingOneChildDirectoryContainingAnotherDirectory_WhenCrawled_Then()
    {
        // arrange

        playgroundDirectory.CreateChildDirectory("Directory1");
        playgroundDirectory.CreateChildDirectory("Directory1", "Directory11");

        // act

        DiskCrawler diskCrawler = new(playgroundDirectory, null, null);
        List<ICrawlerItem> items = diskCrawler.Crawl().ToList();

        // assert

        ExpectedCrawlerItem[] expectedItems =
        {
            new DirectoryOpenedExpectedItem(playgroundDirectory),
            new DirectoryOpenedExpectedItem(playgroundDirectory.CombineWith("Directory1")),
            new DirectoryOpenedExpectedItem(playgroundDirectory.CombineWith("Directory1", "Directory11")),
            new DirectoryClosedExpectedItem(playgroundDirectory.CombineWith("Directory1", "Directory11")),
            new DirectoryClosedExpectedItem(playgroundDirectory.CombineWith("Directory1")),
            new DirectoryClosedExpectedItem(playgroundDirectory)
        };

        CrawlerAssertions.AssertCrawlerItems(expectedItems, items);
    }

    [Fact]
    public void HavingOneChildDirectoryContainingTwoOtherDirectories_WhenCrawled_Then()
    {
        // arrange

        playgroundDirectory.CreateChildDirectory("Directory1");
        playgroundDirectory.CreateChildDirectory("Directory1", "Directory11");
        playgroundDirectory.CreateChildDirectory("Directory1", "Directory12");

        // act

        DiskCrawler diskCrawler = new(playgroundDirectory, null, null);
        List<ICrawlerItem> items = diskCrawler.Crawl().ToList();

        // assert

        ExpectedCrawlerItem[] expectedItems =
        {
            new DirectoryOpenedExpectedItem(playgroundDirectory),
            new DirectoryOpenedExpectedItem(playgroundDirectory.CombineWith("Directory1")),
            new DirectoryOpenedExpectedItem(playgroundDirectory.CombineWith("Directory1", "Directory11")),
            new DirectoryClosedExpectedItem(playgroundDirectory.CombineWith("Directory1", "Directory11")),
            new DirectoryOpenedExpectedItem(playgroundDirectory.CombineWith("Directory1", "Directory12")),
            new DirectoryClosedExpectedItem(playgroundDirectory.CombineWith("Directory1", "Directory12")),
            new DirectoryClosedExpectedItem(playgroundDirectory.CombineWith("Directory1")),
            new DirectoryClosedExpectedItem(playgroundDirectory)
        };

        CrawlerAssertions.AssertCrawlerItems(expectedItems, items);
    }

    [Fact]
    public void HavingOneChildDirectoryContainingOneFile_WhenCrawled_Then()
    {
        // arrange

        playgroundDirectory.CreateChildDirectory("Directory1");
        playgroundDirectory.CreateChildFile("Directory1", "file11.txt");

        // act

        DiskCrawler diskCrawler = new(playgroundDirectory, null, null);
        List<ICrawlerItem> items = diskCrawler.Crawl().ToList();

        // assert

        ExpectedCrawlerItem[] expectedItems =
        {
            new DirectoryOpenedExpectedItem(playgroundDirectory),
            new DirectoryOpenedExpectedItem(playgroundDirectory.CombineWith("Directory1")),
            new FileExpectedItem(playgroundDirectory.CombineWith("Directory1", "file11.txt")),
            new DirectoryClosedExpectedItem(playgroundDirectory.CombineWith("Directory1")),
            new DirectoryClosedExpectedItem(playgroundDirectory)
        };

        CrawlerAssertions.AssertCrawlerItems(expectedItems, items);
    }

    [Fact]
    public void HavingOneChildDirectoryContainingTwoFiles_WhenCrawled_Then()
    {
        // arrange

        playgroundDirectory.CreateChildDirectory("Directory1");
        playgroundDirectory.CreateChildFile("Directory1", "file11.txt");
        playgroundDirectory.CreateChildFile("Directory1", "file12.txt");

        // act

        DiskCrawler diskCrawler = new(playgroundDirectory, null, null);
        List<ICrawlerItem> items = diskCrawler.Crawl().ToList();

        // assert

        ExpectedCrawlerItem[] expectedItems =
        {
            new DirectoryOpenedExpectedItem(playgroundDirectory),
            new DirectoryOpenedExpectedItem(playgroundDirectory.CombineWith("Directory1")),
            new FileExpectedItem(playgroundDirectory.CombineWith("Directory1", "file11.txt")),
            new FileExpectedItem(playgroundDirectory.CombineWith("Directory1", "file12.txt")),
            new DirectoryClosedExpectedItem(playgroundDirectory.CombineWith("Directory1")),
            new DirectoryClosedExpectedItem(playgroundDirectory)
        };

        CrawlerAssertions.AssertCrawlerItems(expectedItems, items);
    }

    [Fact]
    public void HavingTwoChildDirectoriesContainingOneFileEach_WhenCrawled_Then()
    {
        // arrange

        playgroundDirectory.CreateChildDirectory("Directory1");
        playgroundDirectory.CreateChildFile("Directory1", "file11.txt");
        playgroundDirectory.CreateChildDirectory("Directory2");
        playgroundDirectory.CreateChildFile("Directory2", "file21.txt");

        // act

        DiskCrawler diskCrawler = new(playgroundDirectory, null, null);
        List<ICrawlerItem> items = diskCrawler.Crawl().ToList();

        // assert

        ExpectedCrawlerItem[] expectedItems =
        {
            new DirectoryOpenedExpectedItem(playgroundDirectory),
            new DirectoryOpenedExpectedItem(playgroundDirectory.CombineWith("Directory1")),
            new FileExpectedItem(playgroundDirectory.CombineWith("Directory1", "file11.txt")),
            new DirectoryClosedExpectedItem(playgroundDirectory.CombineWith("Directory1")),
            new DirectoryOpenedExpectedItem(playgroundDirectory.CombineWith("Directory2")),
            new FileExpectedItem(playgroundDirectory.CombineWith("Directory2", "file21.txt")),
            new DirectoryClosedExpectedItem(playgroundDirectory.CombineWith("Directory2")),
            new DirectoryClosedExpectedItem(playgroundDirectory)
        };

        CrawlerAssertions.AssertCrawlerItems(expectedItems, items);
    }

    public void Dispose()
    {
        playgroundDirectory.Dispose();
    }
}