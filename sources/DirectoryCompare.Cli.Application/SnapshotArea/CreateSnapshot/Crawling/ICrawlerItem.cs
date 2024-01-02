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

using DustInTheWind.DirectoryCompare.Cli.Application.SnapshotArea.CreateSnapshot.Crawling;
using DustInTheWind.DirectoryCompare.DataStructures;

namespace DustInTheWind.DirectoryCompare.Ports.FileSystemAccess;

internal interface ICrawlerItem
{
    DiskCrawler Owner { get; set; }

    CrawlerAction Action { get; }

    string Name { get; }

    string Path { get; }

    Exception Exception { get; }

    DateTime LastModifiedTime { get; }

    DataSize Size { get; }

    bool IsRoot => Path == Owner?.RootPath;

    Stream ReadContent();
}