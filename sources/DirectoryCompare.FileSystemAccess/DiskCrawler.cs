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
    private readonly IncludeExcludeRuleCollection includeRules;
    private readonly List<string> excludeRules;

    public string RootPath { get; }

    public DiskCrawler(string path, IncludeExcludeRuleCollection includeRules, List<string> excludeRules)
    {
        RootPath = path ?? throw new ArgumentNullException(nameof(path));
        this.includeRules = includeRules ?? throw new ArgumentNullException(nameof(includeRules));
        this.excludeRules = excludeRules ?? throw new ArgumentNullException(nameof(excludeRules));
    }

    public IEnumerable<ICrawlerItem> Crawl()
    {
        if (Directory.Exists(RootPath))
        {
            DirectoryCrawler directoryCrawler = new(RootPath, includeRules, excludeRules, true);
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