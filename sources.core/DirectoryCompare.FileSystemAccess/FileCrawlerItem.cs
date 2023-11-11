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

using DustInTheWind.DirectoryCompare.Domain.Utils;
using DustInTheWind.DirectoryCompare.Ports.FileSystemAccess;

namespace DustInTheWind.DirectoryCompare.FileSystemAccess;

internal class FileCrawlerItem : ICrawlerItem
{
    public CrawlerAction Action { get; } = CrawlerAction.FileFound;

    public string Name => System.IO.Path.GetFileName(Path);

    public string Path { get; }

    public long FileCount { get; }

    public long DirectoryCount { get; }

    public Exception Exception { get; }

    public DateTime LastModifiedTime
    {
        get
        {
            FileInfo fileInfo = new(Path);
            return fileInfo.LastWriteTimeUtc;
        }
    }

    public DataSize Size
    {
        get
        {
            FileInfo fileInfo = new(Path);
            return fileInfo.Length;
        }
    }

    public FileCrawlerItem(string filePath)
    {
        Path = filePath;
    }

    public Stream ReadContent()
    {
        return File.OpenRead(Path);
    }
}