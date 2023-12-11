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

using System.Security.Cryptography;
using DustInTheWind.DirectoryCompare.Ports.DataAccess.ImportExport;
using DustInTheWind.DirectoryCompare.Ports.FileSystemAccess;
using DustInTheWind.DirectoryCompare.Ports.LogAccess;
using DustInTheWind.DirectoryCompare.Ports.UserAccess;

namespace DustInTheWind.DirectoryCompare.Cli.Application.SnapshotArea.CreateSnapshot.DiskAnalysis;

internal sealed class DiskAnalysis : IDisposable
{
    private readonly ILog log;
    private readonly ICreateSnapshotUi createSnapshotUi;
    private readonly MD5 md5;

    private readonly AnalysisProgress analysisProgress;
    private Guid analysisId;

    public IDiskCrawler DiskCrawler { get; init; }

    public PreAnalysis PreAnalysis { get; init; }

    public ISnapshotWriter SnapshotWriter { get; init; }

    public DiskAnalysis(ILog log, ICreateSnapshotUi createSnapshotUi)
    {
        this.log = log ?? throw new ArgumentNullException(nameof(log));
        this.createSnapshotUi = createSnapshotUi ?? throw new ArgumentNullException(nameof(createSnapshotUi));

        md5 = MD5.Create();
        analysisProgress = new AnalysisProgress(createSnapshotUi);
    }

    public Task RunAsync()
    {
        if (DiskCrawler == null)
            throw new Exception("Crawler must be provided to start the analysis.");

        if (SnapshotWriter == null)
            throw new Exception("SnapshotWriter must be provided to start the analysis.");

        if (PreAnalysis == null)
            throw new Exception("PreAnalysis must be provided to start the analysis.");

        return RunInternalAsync();
    }

    private async Task RunInternalAsync()
    {
        try
        {
            await InitializeNewAnalysis();
            
            IEnumerable<IAnalysisItem> analysisItems = GetAnalysisItems();

            foreach (IAnalysisItem analysisItem in analysisItems)
                await ProcessAnalysisItem(analysisItem);
        }
        catch (Exception ex)
        {
            await AnnounceError(ex, null);
        }
        finally
        {
            await analysisProgress.End();
            AnnounceFinished();
        }
    }

    private async Task InitializeNewAnalysis()
    {
        analysisId = Guid.NewGuid();
        await analysisProgress.Start(PreAnalysis.TotalDataSize);

        await AnnounceStarting();
        
        SnapshotWriter?.Open(DiskCrawler.RootPath, analysisId);
    }

    private Task AnnounceStarting()
    {
        return createSnapshotUi.AnnounceAnalysisStarting();
    }

    private IEnumerable<IAnalysisItem> GetAnalysisItems()
    {
        return DiskCrawler.Crawl()
            .Select(CreateAnalysisItem)
            .Where(x => x != null);
    }

    private IAnalysisItem CreateAnalysisItem(ICrawlerItem crawlerItem)
    {
        switch (crawlerItem.Action)
        {
            case CrawlerAction.DirectoryOpened:
                return crawlerItem.IsRoot
                    ? null
                    : new DirectoryOpenedAnalysisItem(crawlerItem);

            case CrawlerAction.DirectoryClosed:
                return crawlerItem.IsRoot
                    ? null
                    : new DirectoryClosedAnalysisItem(crawlerItem);

            case CrawlerAction.FileFound:
                return new FileAnalysisItem(crawlerItem, md5);

            case CrawlerAction.DirectoryError:
                return new ErrorAnalysisItem(crawlerItem);

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task ProcessAnalysisItem(IAnalysisItem analysisItem)
    {
        try
        {
            analysisItem.Analyze();
            analysisItem.Save(SnapshotWriter);

            if (analysisItem.Size > 0)
                await analysisProgress.DoProgress(analysisItem.Size);

            if (analysisItem.Error != null)
            {
                await AnnounceError(analysisItem.Error, analysisItem.Path);
            }
        }
        catch (Exception ex)
        {
            await AnnounceError(ex, analysisItem.Path);
        }
    }

    private Task AnnounceError(Exception exception, string path)
    {
        log.WriteError("Error while reading path '{0}': {1}", path, exception);

        AnalysisErrorInfo info = new(exception, path);
        return createSnapshotUi.AnnounceAnalysisError(info);
    }

    private void AnnounceFinished()
    {
        log.WriteInfo("Finished scanning path in {0}", analysisProgress.Elapsed);

        AnalysisFinishedInfo info = new()
        {
            ElapsedTime = analysisProgress.Elapsed
        };
        createSnapshotUi.AnnounceAnalysisFinished(info);
    }

    public void Dispose()
    {
        md5?.Dispose();
    }
}