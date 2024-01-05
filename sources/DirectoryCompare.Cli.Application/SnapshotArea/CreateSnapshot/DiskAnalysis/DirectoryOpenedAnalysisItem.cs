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
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Ports.DataAccess.ImportExport;
using DustInTheWind.DirectoryCompare.Ports.FileSystemAccess;

namespace DustInTheWind.DirectoryCompare.Cli.Application.SnapshotArea.CreateSnapshot.DiskAnalysis;

internal class DirectoryOpenedAnalysisItem : IAnalysisItem
{
    private readonly ICrawlerItem crawlerItem;
    private HDirectory hDirectory;

    public string Path => crawlerItem.Path;

    public DataSize Size => crawlerItem.Size;

    public Exception Error => crawlerItem.Exception;

    public DirectoryOpenedAnalysisItem(ICrawlerItem crawlerItem)
    {
        this.crawlerItem = crawlerItem ?? throw new ArgumentNullException(nameof(crawlerItem));
    }

    public void Analyze()
    {
        hDirectory = new HDirectory(crawlerItem.Name);
    }

    public void Save(ISnapshotWriter snapshotWriter)
    {
        if (snapshotWriter == null) throw new ArgumentNullException(nameof(snapshotWriter));

        snapshotWriter.AddAndOpen(hDirectory);
    }
}