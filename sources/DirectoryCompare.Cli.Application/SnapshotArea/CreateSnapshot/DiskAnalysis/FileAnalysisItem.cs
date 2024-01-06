// DirectoryCompare
// Copyright (C) 2017-2024 Dust in the Wind
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

using System.Security.Cryptography;
using DustInTheWind.DirectoryCompare.Cli.Application.SnapshotArea.CreateSnapshot.Crawling;
using DustInTheWind.DirectoryCompare.DataStructures;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Ports.DataAccess.ImportExport;
using DustInTheWind.DirectoryCompare.Ports.FileSystemAccess;

namespace DustInTheWind.DirectoryCompare.Cli.Application.SnapshotArea.CreateSnapshot.DiskAnalysis;

internal class FileAnalysisItem : IAnalysisItem
{
    private readonly ICrawlerItem crawlerItem;
    private readonly MD5 md5;
    private HFile hFile;

    public string Path => crawlerItem.Path;

    public DataSize Size => crawlerItem.Size;

    public Exception Error { get; private set; }

    public FileAnalysisItem(ICrawlerItem crawlerItem, MD5 md5)
    {
        this.crawlerItem = crawlerItem ?? throw new ArgumentNullException(nameof(crawlerItem));
        this.md5 = md5 ?? throw new ArgumentNullException(nameof(md5));
    }

    public void Analyze()
    {
        try
        {
            hFile = new HFile
            {
                Name = crawlerItem.Name,
                LastModifiedTime = crawlerItem.LastModifiedTime
            };

            using Stream stream = crawlerItem.ReadContent();

            hFile.Hash = md5.ComputeHash(stream);
            hFile.Size = stream.Length;
        }
        catch (Exception ex)
        {
            Error = ex;
            hFile.Error = ex.Message;
        }
    }

    public void Save(ISnapshotWriter snapshotWriter)
    {
        snapshotWriter?.Add(hFile);
    }
}