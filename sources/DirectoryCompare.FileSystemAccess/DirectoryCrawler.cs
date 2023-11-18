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

using System.Collections;
using DustInTheWind.DirectoryCompare.Ports.FileSystemAccess;

namespace DustInTheWind.DirectoryCompare.FileSystemAccess;

internal class DirectoryCrawler
{
    private readonly string path;
    private readonly List<string> blackList;

    private string[] filePaths;
    private string[] directoryPaths;
    private Exception exception;

    public DirectoryCrawler(string path, List<string> blackList)
    {
        this.path = path ?? throw new ArgumentNullException(nameof(path));
        this.blackList = blackList ?? throw new ArgumentNullException(nameof(blackList));
    }

    public IEnumerable<ICrawlerItem> Crawl()
    {
        OpenDirectory();

        if (exception != null)
        {
            yield return new DirectoryErrorCrawlerItem(exception, path);
        }
        else
        {
            yield return new DirectoryOpenCrawlerItem(path, filePaths.Length, directoryPaths.Length);

            foreach (string filePath in filePaths)
                yield return new FileCrawlerItem(filePath);

            IEnumerable<ICrawlerItem> crawlerItems = directoryPaths
                .Select(x =>
                {
                    DirectoryCrawler directoryCrawler = new(x, blackList);
                    return directoryCrawler.Crawl();
                })
                .SelectMany(x => x);

            foreach (ICrawlerItem crawlerItem in crawlerItems)
                yield return crawlerItem;

            yield return new DirectoryCloseCrawlerItem(path);
        }
    }

    private void OpenDirectory()
    {
        filePaths = null;
        directoryPaths = null;
        exception = null;

        try
        {
            filePaths = Directory.GetFiles(path)
                .Where(x => !blackList.Contains(x))
                .ToArray();

            directoryPaths = Directory.GetDirectories(path)
                .Where(x => !blackList.Contains(x))
                .ToArray();
        }
        catch (Exception ex)
        {
            exception = ex;
        }
    }
}