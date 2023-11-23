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

using System.Diagnostics;
using System.Security.Cryptography;
using DustInTheWind.DirectoryCompare.DataStructures;
using DustInTheWind.DirectoryCompare.Domain.PotModel;
using DustInTheWind.DirectoryCompare.Domain.Utils;
using DustInTheWind.DirectoryCompare.Ports.DataAccess;
using DustInTheWind.DirectoryCompare.Ports.DataAccess.ImportExport;
using DustInTheWind.DirectoryCompare.Ports.FileSystemAccess;
using DustInTheWind.DirectoryCompare.Ports.LogAccess;
using DustInTheWind.DirectoryCompare.Ports.SystemAccess;
using DustInTheWind.DirectoryCompare.Ports.UserAccess;

namespace DustInTheWind.DirectoryCompare.Cli.Application.SnapshotArea.CreateSnapshot.DiskAnalysis;

internal sealed class DiskAnalysis : IDisposable
{
    private readonly ILog log;
    private readonly IFileSystem fileSystem;
    private readonly ISnapshotRepository snapshotRepository;
    private readonly ICreateSnapshotUserInterface createSnapshotUserInterface;
    private readonly ISystemClock systemClock;
    private readonly Stopwatch stopwatch = new();
    private readonly MD5 md5;
    private ISnapshotWriter snapshotWriter;

    private Progress progress;
    private Guid analysisId;
    private float lastPercentageAnnounced;
    private DiskAnalysisReport report;

    public Pot Pot { get; init; }

    public DiskPathCollection BlackList { get; init; }

    public DiskAnalysis(ILog log, IFileSystem fileSystem, ISnapshotRepository snapshotRepository,
        ICreateSnapshotUserInterface createSnapshotUserInterface, ISystemClock systemClock)
    {
        this.log = log ?? throw new ArgumentNullException(nameof(log));
        this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
        this.createSnapshotUserInterface = createSnapshotUserInterface ?? throw new ArgumentNullException(nameof(createSnapshotUserInterface));
        this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));

        md5 = MD5.Create();
    }

    public DiskAnalysisReport Start()
    {
        ResetAnalysis();

        _ = Task.Run(async () =>
        {
            try
            {
                AnnounceStarting();

                DataSize totalSize = await CalculateTotalSize();
                progress = new Progress(totalSize);

                snapshotWriter = await snapshotRepository.CreateWriter(Pot.Name);
                snapshotWriter?.Open(Pot.Path, analysisId);

                AnnounceProgress();
                await CalculateHashes();

                if (Math.Abs(lastPercentageAnnounced - 100) > float.Epsilon)
                    AnnounceProgress();
            }
            catch (Exception ex)
            {
                AnnounceError(ex, null);
            }
            finally
            {
                ConcludeAnalysis();
                AnnounceFinished();
            }
        });

        return report;
    }

    private void ResetAnalysis()
    {
        stopwatch.Start();
        analysisId = Guid.NewGuid();
        progress = null;
        lastPercentageAnnounced = 0;
        report = new DiskAnalysisReport();
    }

    private void ConcludeAnalysis()
    {
        stopwatch.Stop();
    }

    private Task<DataSize> CalculateTotalSize()
    {
        return Task.Run(() =>
        {
            IDiskCrawler diskCrawler = fileSystem.CreateCrawler(Pot.Path, BlackList.ToListOfStrings());

            long dataSize = diskCrawler.Crawl()
                .AsParallel()
                .Where(x => x.Action == CrawlerAction.FileFound)
                .Select(x => (long)(ulong)x.Size)
                .Sum();

            return (DataSize)dataSize;
        });
    }

    private Task CalculateHashes()
    {
        return Task.Run(() =>
        {
            IDiskCrawler diskCrawler = fileSystem.CreateCrawler(Pot.Path, BlackList.ToListOfStrings());

            IEnumerable<IAnalysisItem> analysisItems = diskCrawler.Crawl()
                .Select(CreateAnalysisItem)
                .Where(x => x != null);

            foreach (IAnalysisItem analysisItem in analysisItems)
                ProcessAnalysisItem(analysisItem);
        });
    }

    private IAnalysisItem CreateAnalysisItem(ICrawlerItem crawlerItem)
    {
        switch (crawlerItem.Action)
        {
            case CrawlerAction.DirectoryOpened:
                return crawlerItem.Path != Pot.Path
                    ? new DirectoryOpenedAnalysisItem(crawlerItem)
                    : null;

            case CrawlerAction.DirectoryClosed:
                return crawlerItem.Path != Pot.Path
                    ? new DirectoryClosedAnalysisItem(crawlerItem)
                    : null;

            case CrawlerAction.FileFound:
                return new FileAnalysisItem(crawlerItem, md5);

            case CrawlerAction.DirectoryError:
                return new ErrorAnalysisItem(crawlerItem);

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ProcessAnalysisItem(IAnalysisItem analysisItem)
    {
        analysisItem.Analyze();
        analysisItem.Save(snapshotWriter);

        if (progress != null && analysisItem.Size > 0)
        {
            progress.Value += analysisItem.Size;
            AnnounceProgress();
        }

        if (analysisItem.Error != null)
        {
            AnnounceError(analysisItem.Error, analysisItem.Path);
        }
    }

    private void AnnounceStarting()
    {
        log.WriteInfo("Scanning path: {0}", Pot.Path);

        if (BlackList.Count == 0)
        {
            log.WriteInfo("No blacklist entries.");
            return;
        }

        log.WriteInfo("Computed black list:");

        foreach (string blackListItem in BlackList)
            log.WriteInfo("- " + blackListItem);

        DiskReaderStartingEventArgs eventArgs = new(BlackList);
        report.OnStarting(eventArgs);
    }

    private async Task AnnounceSnapshotCreating()
    {
        StartNewSnapshotInfo info = new()
        {
            PotName = Pot.Name,
            Path = Pot.Path,
            BlackList = BlackList
                .Select(x => x.ToString())
                .ToList(),
            StartTime = systemClock.GetCurrentUtcTime()
        };
        
        await createSnapshotUserInterface.AnnounceSnapshotCreating(info);
    }

    private void AnnounceFinished()
    {
        log.WriteInfo("Finished scanning path in {0}", stopwatch.Elapsed);
        snapshotWriter.Dispose();

        report.OnFinished();
    }

    private void AnnounceProgress()
    {
        float currentPercentage = progress.Percentage;
        float percentageDifference = currentPercentage - lastPercentageAnnounced;

        if (percentageDifference < 0.1)
            return;

        DiskAnalysisProgressEventArgs args = new()
        {
            Percentage = progress,
            TotalSize = progress.Size,
            ProcessedSize = progress.Value - progress.MinValue,
            ElapsedTime = stopwatch.Elapsed
        };
        report.OnProgress(args);

        lastPercentageAnnounced = currentPercentage;
    }

    private void AnnounceError(Exception ex, string path)
    {
        log.WriteError("Error while reading path '{0}': {1}", path, ex);

        ErrorEncounteredEventArgs eventArgs = new(ex, path);
        report.OnErrorEncountered(eventArgs);
    }

    public void Dispose()
    {
        md5?.Dispose();
    }
}