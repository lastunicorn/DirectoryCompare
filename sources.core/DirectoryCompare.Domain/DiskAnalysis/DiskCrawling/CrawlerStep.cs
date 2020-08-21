// DirectoryCompare
// Copyright (C) 2017-2020 Dust in the Wind
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

using System;

namespace DustInTheWind.DirectoryCompare.Domain.DiskAnalysis.DiskCrawling
{
    internal struct CrawlerStep
    {
        public CrawlerAction Action { get; private set; }
        
        public string Path { get; private set; }
        
        public long FileCount { get; private set; }
        
        public long DirectoryCount { get; private set; }
        
        public Exception Exception { get; private set; }

        public static CrawlerStep DirectoryOpened(string directoryPath, long fileCount, long directoryCount)
        {
            return new CrawlerStep
            {
                Action = CrawlerAction.DirectoryOpened,
                Path = directoryPath,
                FileCount = fileCount,
                DirectoryCount = directoryCount
            };
        }

        public static CrawlerStep DirectoryClosed(string directoryPath)
        {
            return new CrawlerStep
            {
                Action = CrawlerAction.DirectoryClosed,
                Path = directoryPath
            };
        }

        public static CrawlerStep FileFound(string filePath)
        {
            return new CrawlerStep
            {
                Action = CrawlerAction.FileFound,
                Path = filePath
            };
        }

        public static CrawlerStep Error(Exception exception, string itemName)
        {
            return new CrawlerStep
            {
                Action = CrawlerAction.Error,
                Exception = exception,
                Path = itemName
            };
        }
    }
}