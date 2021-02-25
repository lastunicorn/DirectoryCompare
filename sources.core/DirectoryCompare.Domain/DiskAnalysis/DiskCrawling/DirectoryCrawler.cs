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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DustInTheWind.DirectoryCompare.Domain.Utils;

namespace DustInTheWind.DirectoryCompare.Domain.DiskAnalysis.DiskCrawling
{
    internal class DirectoryCrawler : IEnumerable<CrawlerStep>
    {
        private readonly string path;
        private readonly DiskPathCollection blackList;

        private string[] filePaths;
        private string[] directoryPaths;
        private Exception exception;

        public DirectoryCrawler(string path, DiskPathCollection blackList)
        {
            this.path = path ?? throw new ArgumentNullException(nameof(path));
            this.blackList = blackList ?? throw new ArgumentNullException(nameof(blackList));
        }

        public IEnumerator<CrawlerStep> GetEnumerator()
        {
            OpenDirectory();

            if (exception != null)
            {
                yield return CrawlerStep.Error(exception, path);
            }
            else
            {
                yield return CrawlerStep.DirectoryOpened(path, filePaths.Length, directoryPaths.Length);

                foreach (string filePath in filePaths)
                    yield return CrawlerStep.FileFound(filePath);

                IEnumerable<CrawlerStep> steps = directoryPaths
                    .Select(x => new DirectoryCrawler(x, blackList))
                    .SelectMany(x => x);

                foreach (CrawlerStep crawlerStep in steps)
                    yield return crawlerStep;

                yield return CrawlerStep.DirectoryClosed(path);
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}