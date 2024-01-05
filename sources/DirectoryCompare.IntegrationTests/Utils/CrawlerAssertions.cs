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

using DustInTheWind.DirectoryCompare.Cli.Application.SnapshotArea.CreateSnapshot.Crawling;
using DustInTheWind.DirectoryCompare.Ports.FileSystemAccess;
using FluentAssertions;
using FluentAssertions.Execution;

namespace DustInTheWind.DirectoryCompare.IntegrationTests.Utils;

internal static class CrawlerAssertions
{
    public static void AssertCrawlerItems(IEnumerable<ExpectedCrawlerItem> expectedItems, IEnumerable<ICrawlerItem> actualItems)
    {
        using IEnumerator<ExpectedCrawlerItem> expectedEnumerator = expectedItems.GetEnumerator();
        using IEnumerator<ICrawlerItem> actualEnumerator = actualItems.GetEnumerator();

        while (expectedEnumerator.MoveNext())
        {
            if (!actualEnumerator.MoveNext())
                throw new AssertionFailedException("Actual items are less than expected.");

            actualEnumerator.Current.Action.Should().Be(expectedEnumerator.Current.Action);
            actualEnumerator.Current.Path.Should().Be(expectedEnumerator.Current.Path);
        }

        if (actualEnumerator.MoveNext())
            throw new AssertionFailedException("Actual items are more than expected.");
    }
}