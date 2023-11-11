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

using System.Collections;
using DustInTheWind.DirectoryCompare.Domain.Utils;
using DustInTheWind.DirectoryCompare.Ports.FileSystemAccess;

namespace DustInTheWind.DirectoryCompare.FileSystemAccess;

internal class DiskCrawler : IDiskCrawler
{
    private readonly string path;
    private readonly DiskPathCollection blackList;

    public DiskCrawler(string path, DiskPathCollection blackList)
    {
        this.path = path ?? throw new ArgumentNullException(nameof(path));
        this.blackList = blackList ?? throw new ArgumentNullException(nameof(blackList));
    }

    public IEnumerator<ICrawlerItem> GetEnumerator()
    {
        if (!Directory.Exists(path))
        {
            Exception exception = new($"The path '{path}' does not exist.");
            yield return new ErrorCrawlerItem(exception, path);
        }
        else
        {
            DirectoryCrawler directoryCrawler = new(path, blackList);

            foreach (ICrawlerItem crawlerItem in directoryCrawler)
                yield return crawlerItem;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}