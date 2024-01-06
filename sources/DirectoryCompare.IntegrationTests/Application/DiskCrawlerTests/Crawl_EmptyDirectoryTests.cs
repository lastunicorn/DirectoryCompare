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

using DustInTheWind.DirectoryCompare.Cli.Application.SnapshotArea.CreateSnapshot.Crawling;
using DustInTheWind.DirectoryCompare.FileSystemAccess;
using DustInTheWind.DirectoryCompare.IntegrationTests.Utils;
using DustInTheWind.DirectoryCompare.Ports.FileSystemAccess;
using Xunit;

namespace DustInTheWind.DirectoryCompare.IntegrationTests.Application.DiskCrawlerTests;

[Collection("CrawlerTests")]
public class Crawl_EmptyDirectoryTests : IDisposable
{
    private readonly PlaygroundDirectory playgroundDirectory = new();

    [Fact]
    public void HavingEmptyDirectory_WhenCrawledWithExactMatchFlag_ThenReturnsOpenAndCloseItems()
    {
        // arrange

        FileSystem fileSystem = new();

        // act

        DiskCrawler diskCrawler = new(playgroundDirectory, null, null, fileSystem);
        List<ICrawlerItem> items = diskCrawler.Crawl().ToList();

        // assert

        ExpectedCrawlerItem[] expectedItems =
        {
            new DirectoryOpenedExpectedItem(playgroundDirectory),
            new DirectoryClosedExpectedItem(playgroundDirectory)
        };

        CrawlerAssertions.AssertCrawlerItems(expectedItems, items);
    }

    public void Dispose()
    {
        playgroundDirectory.Dispose();
    }
}