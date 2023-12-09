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
        this.includeRules = includeRules;
        this.excludeRules = excludeRules;
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

    //private IEnumerable<ICrawlerItem> ProcessDirectory(CrawlRequest request)
    //{
    //    string[] filePaths = null;
    //    string[] directoryPaths = null;
    //    Exception exception = null;

    //    try
    //    {
    //        filePaths = Directory.GetFiles(request.Path);
    //        directoryPaths = Directory.GetDirectories(request.Path);
    //    }
    //    catch (Exception ex)
    //    {
    //        exception = ex;
    //    }

    //    if (exception != null)
    //    {
    //        yield return new DirectoryErrorCrawlerItem(exception, request.Path);
    //    }
    //    else
    //    {
    //        CrawlTask crawlTask = new()
    //        {
    //            Request = request,
    //            Files = filePaths,
    //            Diretories = directoryPaths
    //        };

    //        bool isDirectoryOpened = false;

    //        // Open Directory

    //        if (request.IsExactMatch)
    //        {
    //            yield return new DirectoryOpenCrawlerItem(request.Path);
    //            isDirectoryOpened = true;
    //        }

    //        // Process Files

    //        IEnumerable<ICrawlerItem> childItems = crawlTask.EnumerateItems;

    //        foreach (ICrawlerItem item in childItems)
    //        {
    //            if (!isDirectoryOpened)
    //            {
    //                yield return new DirectoryOpenCrawlerItem(request.Path);
    //                isDirectoryOpened = true;
    //            }
                
    //            yield return item;
    //        }

    //        // Process Sub-Directories

    //        IEnumerable<CrawlRequest> requests = crawlTask.EnumerateRequests;

    //        foreach (CrawlRequest childRequest in requests)
    //        {
    //            IEnumerable<ICrawlerItem> items = ProcessDirectory(request);

    //            foreach (ICrawlerItem item in items)
    //            {
    //                if (!isDirectoryOpened)
    //                {
    //                    yield return new DirectoryOpenCrawlerItem(request.Path);
    //                    isDirectoryOpened = true;
    //                }

    //                yield return item;
    //            }
    //        }

    //        // Close Directory

    //        if (isDirectoryOpened)
    //            yield return new DirectoryCloseCrawlerItem(request.Path);
    //    }
    //}
}

//internal struct CrawlRequest
//{
//    public string Path { get; set; }

//    public IncludeExcludeRuleCollection IncludeRules { get; set; }

//    public List<string> ExcludeRules { get; set; }

//    public bool IsExactMatch { get; set; }
//}

//internal class CrawlerTask
//{
//    public string Path { get; set; }

//    public IncludeExcludeRuleCollection IncludeRules { get; set; }

//    public List<string> ExcludeRules { get; set; }

//    public bool IsExactMatch { get; set; }

//    public string[] FilePaths { get; set; }

//    public string[] DirectoryPaths { get; set; }

//    public IEnumerable<ICrawlerItem> Crawl()
//    {
//        OpenDirectory();

//        if (exception != null)
//        {
//            yield return new DirectoryErrorCrawlerItem(exception, Path);
//        }
//        else
//        {
//            bool isDirectoryOpened = false;

//            if (IsExactMatch)
//            {
//                yield return new DirectoryOpenCrawlerItem(Path);
//                isDirectoryOpened = true;
//            }

//            // Process Files

//            IEnumerable<FileCrawlerItem> fileCrawlerItems = FilePaths
//                .Select(ProcessFile)
//                .Where(x => x != null);

//            foreach (FileCrawlerItem fileCrawlerItem in fileCrawlerItems)
//            {
//                if (!isDirectoryOpened)
//                {
//                    yield return new DirectoryOpenCrawlerItem(Path);
//                    isDirectoryOpened = true;
//                }

//                yield return fileCrawlerItem;
//            }

//            // Process Sub-Directories

//            IEnumerable<ICrawlerItem> crawlerItems = DirectoryPaths
//                .SelectMany(ProcessDirectory);

//            foreach (ICrawlerItem crawlerItem in crawlerItems)
//            {
//                if (!isDirectoryOpened)
//                {
//                    yield return new DirectoryOpenCrawlerItem(Path);
//                    isDirectoryOpened = true;
//                }

//                yield return crawlerItem;
//            }

//            if (isDirectoryOpened)
//                yield return new DirectoryCloseCrawlerItem(Path);
//        }
//    }

//    private FileCrawlerItem ProcessFile(string filePath)
//    {
//        string fileName = Path.GetFileName(filePath);

//        // Is Excluded

//        bool isExcluded = ExcludeRules.Contains(fileName);

//        if (isExcluded)
//            return null;

//        // Is Included

//        IncludeExcludeMatchCollection matches = IncludeRules.Match(fileName);
//        matches.Analyze(false);

//        return matches.IsExactMatch
//            ? new FileCrawlerItem(filePath)
//            : null;
//    }

//    private IEnumerable<ICrawlerItem> ProcessDirectory(string directoryPath)
//    {
//        string directoryName = Path.GetFileName(directoryPath);

//        // Is Excluded

//        bool isExcluded = ExcludeRules.Contains(directoryName);

//        if (isExcluded)
//            return Enumerable.Empty<ICrawlerItem>();

//        // Is Included

//        IncludeExcludeMatchCollection matches = IncludeRules.Match(directoryName);
//        matches.Analyze();

//        if (matches.IsExactMatch)
//        {
//            DirectoryCrawler directoryCrawler = new(directoryPath, new IncludeExcludeRuleCollection(), ExcludeRules, true);
//            return directoryCrawler.Crawl();
//        }
//        else if (matches.IsIntermediateMatch)
//        {
//            // todo: current dir should not be "opened" until a children is proven to be green.

//            DirectoryCrawler directoryCrawler = new(directoryPath, matches.NextRules, ExcludeRules, false);
//            return directoryCrawler.Crawl();
//        }
//        else if (matches.NextRules.Count > 0)
//        {
//            // todo: current dir should not be "opened" until a children is proven to be green.

//            DirectoryCrawler directoryCrawler = new(directoryPath, matches.NextRules, ExcludeRules, false);
//            return directoryCrawler.Crawl();
//        }
//        else
//        {
//            return Enumerable.Empty<ICrawlerItem>();
//        }
//    }
//}