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
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Domain.Utils;
using DustInTheWind.DirectoryCompare.Ports.DataAccess.ImportExport;
using DustInTheWind.DirectoryCompare.Ports.FileSystemAccess;

namespace DustInTheWind.DirectoryCompare.Cli.Application.SnapshotArea.CreateSnapshot.DiskAnalysis;

public sealed class DiskAnalysis : IDisposable
{
    private readonly IFileSystem fileSystem;
    private readonly Stopwatch stopwatch = new();
    private readonly MD5 md5;

    private string rootPath;
    private Progress progress;
    private Guid analysisId;
    private float lastPercentageAnnounced;
    private DiskAnalysisStateReport stateReport = new();

    public IDiskAnalysisStateReport StateReport => stateReport;

    public string RootPath
    {
        get => rootPath;
        set => rootPath = value ?? string.Empty;
    }

    public ISnapshotWriter SnapshotWriter { get; set; }

    public DiskPathCollection BlackList { get; set; }

    public TimeSpan ElapsedTime => stopwatch.Elapsed;

    public DiskAnalysis(IFileSystem fileSystem)
    {
        this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        md5 = MD5.Create();
    }

    public async Task Run()
    {
        ResetAnalysis();

        try
        {
            DiskReaderStartingEventArgs eventArgs = new(BlackList);
            stateReport.OnStarting(eventArgs);

            DataSize totalSize = await CalculateTotalSize();
            progress = new Progress(totalSize);

            SnapshotWriter?.Open(RootPath, analysisId);

            AnnounceProgress();
            await CalculateHashes();
            if (Math.Abs(lastPercentageAnnounced - 100) > float.Epsilon)
                AnnounceProgress();

            SnapshotWriter?.Close();
        }
        catch (Exception ex)
        {
            ErrorEncounteredEventArgs eventArgs = new(ex, null);
            stateReport.OnErrorEncountered(eventArgs);
        }
        finally
        {
            ConcludeAnalysis();
        }
    }

    private void ResetAnalysis()
    {
        stopwatch.Start();
        progress = null;
        analysisId = Guid.NewGuid();
        lastPercentageAnnounced = 0;
    }

    private void ConcludeAnalysis()
    {
        stopwatch.Stop();
        stateReport.OnFinished();
    }

    private Task<DataSize> CalculateTotalSize()
    {
        return Task.Run(() =>
        {
            IDiskCrawler diskCrawler = fileSystem.CreateCrawler(RootPath, BlackList.ToListOfStrings());

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
            IDiskCrawler diskCrawler = fileSystem.CreateCrawler(RootPath, BlackList.ToListOfStrings());
            IEnumerable<ICrawlerItem> crawlerItems = diskCrawler.Crawl();
            
            foreach (ICrawlerItem crawlerItem in crawlerItems)
            {
                switch (crawlerItem.Action)
                {
                    case CrawlerAction.DirectoryOpened:
                        if (crawlerItem.Path != RootPath)
                            AddDirectory(crawlerItem);
                        break;

                    case CrawlerAction.DirectoryClosed:
                        if (crawlerItem.Path != RootPath)
                            CloseDirectory();
                        break;

                    case CrawlerAction.FileFound:
                        AddFile(crawlerItem);
                        break;

                    case CrawlerAction.Error:
                        ProcessError(crawlerItem);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        });
    }

    private void AddDirectory(ICrawlerItem crawlerItem)
    {
        HDirectory hDirectory = new(crawlerItem.Name);
        SnapshotWriter?.AddAndOpen(hDirectory);
    }

    private void CloseDirectory()
    {
        SnapshotWriter?.CloseDirectory();
    }

    private void AddFile(ICrawlerItem crawlerItem)
    {
        HFile hFile = AnalyzeFile(crawlerItem);
        SnapshotWriter?.Add(hFile);

        if (progress != null)
            UpdateProgress(hFile.Size);
    }

    private HFile AnalyzeFile(ICrawlerItem crawlerItem)
    {
        HFile hFile = new()
        {
            Name = crawlerItem.Name
        };

        try
        {
            hFile.LastModifiedTime = crawlerItem.LastModifiedTime;

            using Stream stream = crawlerItem.ReadContent();

            hFile.Hash = md5.ComputeHash(stream);
            long size = stream.Length;

            hFile.Size = size;
        }
        catch (Exception ex)
        {
            ErrorEncounteredEventArgs eventArgs = new(ex, crawlerItem.Path);
            stateReport.OnErrorEncountered(eventArgs);
            
            hFile.Error = ex.Message;
        }

        return hFile;
    }

    private void UpdateProgress(DataSize dataSize)
    {
        progress.Value += dataSize;
        float currentPercentageValue = progress.Percentage;

        if (Math.Abs(lastPercentageAnnounced - currentPercentageValue) >= 0.1)
            AnnounceProgress();
    }

    private void AnnounceProgress()
    {
        DiskAnalysisProgressEventArgs args = new()
        {
            Percentage = progress,
            TotalSize = progress.Size,
            ProcessedSize = progress.Value - progress.MinValue,
            ElapsedTime = stopwatch.Elapsed
        };
        stateReport.OnProgress(args);

        lastPercentageAnnounced = progress.Percentage;
    }

    private void ProcessError(ICrawlerItem crawlerItem)
    {
        ErrorEncounteredEventArgs args = new(crawlerItem.Exception, crawlerItem.Path);
        stateReport.OnErrorEncountered(args);

        HDirectory hDirectory = new()
        {
            Name = crawlerItem.Name,
            Error = crawlerItem.Exception.Message
        };

        SnapshotWriter?.Add(hDirectory);
    }

    public void Dispose()
    {
        md5?.Dispose();
    }
}