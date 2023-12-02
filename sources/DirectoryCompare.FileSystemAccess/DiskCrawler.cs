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

using DustInTheWind.DirectoryCompare.Ports.FileSystemAccess;

namespace DustInTheWind.DirectoryCompare.FileSystemAccess;

internal class DiskCrawler : IDiskCrawler
{
    private readonly List<string> blackList;

    public string RootPath { get; }

    public DiskCrawler(string path, List<string> blackList)
    {
        RootPath = path ?? throw new ArgumentNullException(nameof(path));
        this.blackList = blackList ?? throw new ArgumentNullException(nameof(blackList));
    }

    public IEnumerable<ICrawlerItem> Crawl()
    {
        if (Directory.Exists(RootPath))
        {
            DirectoryCrawler directoryCrawler = new(RootPath, blackList);
            IEnumerable<ICrawlerItem> crawlerItems = directoryCrawler.Crawl();

            foreach (ICrawlerItem crawlerItem in crawlerItems)
            {
                crawlerItem.Owner = this;
                yield return crawlerItem;
            }
        }
        else
        {
            Exception exception = new($"The path '{RootPath}' does not exist.");
            yield return new DirectoryErrorCrawlerItem(exception, RootPath)
            {
                Owner = this
            };
        }
    }
}