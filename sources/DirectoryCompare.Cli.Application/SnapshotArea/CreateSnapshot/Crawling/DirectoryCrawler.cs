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

namespace DustInTheWind.DirectoryCompare.Cli.Application.SnapshotArea.CreateSnapshot.Crawling;

internal class DirectoryCrawler
{
    private readonly string path;
    private readonly IncludeExcludeRuleCollection includeRules;
    private readonly List<string> excludeRules;
    private readonly bool isExactMatch;
    private readonly IFileSystem fileSystem;

    private string[] filePaths;
    private string[] directoryPaths;
    private Exception exception;

    public DirectoryCrawler(string path, IncludeExcludeRuleCollection includeRules, List<string> excludeRules, bool isExactMatch, IFileSystem fileSystem)
    {
        this.path = path ?? throw new ArgumentNullException(nameof(path));
        this.includeRules = includeRules;
        this.excludeRules = excludeRules;
        this.isExactMatch = isExactMatch;
        this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
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
            bool isDirectoryOpened = false;

            if (isExactMatch)
            {
                yield return new DirectoryOpenCrawlerItem(path);
                isDirectoryOpened = true;
            }

            // Process Files

            IEnumerable<FileCrawlerItem> fileCrawlerItems = filePaths
                .Select(ProcessFile)
                .Where(x => x != null);

            foreach (FileCrawlerItem fileCrawlerItem in fileCrawlerItems)
            {
                if (!isDirectoryOpened)
                {
                    yield return new DirectoryOpenCrawlerItem(path);
                    isDirectoryOpened = true;
                }

                yield return fileCrawlerItem;
            }

            // Process Sub-Directories

            IEnumerable<ICrawlerItem> crawlerItems = directoryPaths
                .SelectMany(ProcessDirectory);

            foreach (ICrawlerItem crawlerItem in crawlerItems)
            {
                if (!isDirectoryOpened)
                {
                    yield return new DirectoryOpenCrawlerItem(path);
                    isDirectoryOpened = true;
                }

                yield return crawlerItem;
            }

            if (isDirectoryOpened)
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
            filePaths = fileSystem.GetFiles(path);
            directoryPaths = fileSystem.GetDirectories(path);
        }
        catch (Exception ex)
        {
            exception = ex;
        }
    }

    private FileCrawlerItem ProcessFile(string filePath)
    {
        string fileName = Path.GetFileName(filePath);

        // Is Excluded

        bool isExcluded = excludeRules?.Contains(fileName) ?? false;

        if (isExcluded)
            return null;

        // Is Included

        IncludeExcludeMatchCollection matches = includeRules?.Match(fileName) ?? new IncludeExcludeMatchCollection();
        matches.Analyze(false);

        return matches.IsExactMatch
            ? new FileCrawlerItem(filePath)
            : null;
    }

    private IEnumerable<ICrawlerItem> ProcessDirectory(string directoryPath)
    {
        string directoryName = Path.GetFileName(directoryPath);

        // Is Excluded

        bool isExcluded = excludeRules?.Contains(directoryName) ?? false;

        if (isExcluded)
            return Enumerable.Empty<ICrawlerItem>();

        // Is Included

        IncludeExcludeMatchCollection matches = includeRules?.Match(directoryName) ?? new IncludeExcludeMatchCollection();
        matches.Analyze();

        if (matches.IsExactMatch)
        {
            DirectoryCrawler directoryCrawler = new(directoryPath, new IncludeExcludeRuleCollection(), excludeRules, true, fileSystem);
            return directoryCrawler.Crawl();
        }
        else if (matches.IsIntermediateMatch)
        {
            // todo: current dir should not be "opened" until a children is proven to be green.

            DirectoryCrawler directoryCrawler = new(directoryPath, matches.NextRules, excludeRules, false, fileSystem);
            return directoryCrawler.Crawl();
        }
        else if(matches.NextRules.Count > 0)
        {
            // todo: current dir should not be "opened" until a children is proven to be green.

            DirectoryCrawler directoryCrawler = new(directoryPath, matches.NextRules, excludeRules, false, fileSystem);
            return directoryCrawler.Crawl();
        }
        else
        {
            return Enumerable.Empty<ICrawlerItem>();
        }
    }
}