// DirectoryCompare
// Copyright (C) 2017 Dust in the Wind
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DustInTheWind.DirectoryCompare.Utils;

namespace DustInTheWind.DirectoryCompare.DiskCrawling
{
    public class DiskCrawler : IEnumerable<CrawlerStep>
    {
        private readonly string path;
        private readonly PathCollection blackList;

        public DiskCrawler(string path, PathCollection blackList)
        {
            this.path = path ?? throw new ArgumentNullException(nameof(path));
            this.blackList = blackList ?? throw new ArgumentNullException(nameof(blackList));
        }

        public IEnumerator<CrawlerStep> GetEnumerator()
        {
            if (!Directory.Exists(path))
            {
                Exception exception = new Exception($"The path '{path}' does not exist.");
                yield return CrawlerStep.Error(exception, path);
            }
            else
            {
                DirectoryCrawler directoryCrawler = new DirectoryCrawler(path, blackList);

                foreach (CrawlerStep crawlerStep in directoryCrawler)
                    yield return crawlerStep;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}