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

using System.Diagnostics;
using System.Security.Cryptography;
using DustInTheWind.DirectoryCompare.DiskAnalysis.DiskCrawling;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Domain.ImportExport;
using DustInTheWind.DirectoryCompare.Domain.Utils;

namespace DustInTheWind.DirectoryCompare.DiskAnalysis;

public sealed class DiskAnalysis : IDiskAnalysisProgress, IDisposable
{
    private readonly Stopwatch stopwatch = new();
    private readonly ManualResetEventSlim manualResetEventSlim = new(false);
    private readonly MD5 md5;

    private string rootPath;
    private Percentage progressPercentage;
    private Guid analysisId;
    private DiskPathCollection rootedBlackList;

    public string RootPath
    {
        get => rootPath;
        set => rootPath = value ?? string.Empty;
    }

    public ISnapshotWriter SnapshotWriter { get; set; }

    public DiskPathCollection BlackList { get; set; }

    public DiskAnalysisState State { get; private set; }

    public event EventHandler<ErrorEncounteredEventArgs> ErrorEncountered;
    public event EventHandler<DiskReaderStartingEventArgs> Starting;
    public event EventHandler<DiskAnalysisProgressEventArgs> Progress;
    public event EventHandler Finished;

    public TimeSpan ElapsedTime => stopwatch.Elapsed;

    public DiskAnalysis()
    {
        md5 = MD5.Create();
    }

    public async Task Run()
    {
        ResetAnalysis();

        try
        {
            rootedBlackList = BlackList?.PrependPath(RootPath) ?? new DiskPathCollection();
            OnStarting(new DiskReaderStartingEventArgs(rootedBlackList));

            SnapshotWriter?.Open(RootPath, analysisId);

            DataSize totalSize = await CalculateTotalSize();
            progressPercentage = new Percentage(totalSize);

            await CalculateHashes();

            SnapshotWriter?.Close();
        }
        finally
        {
            ConcludeAnalysis();
        }
    }

    private void ResetAnalysis()
    {
        if (State == DiskAnalysisState.InProgress)
            throw new AnalysisInProgressException();

        State = DiskAnalysisState.InProgress;

        stopwatch.Start();
        manualResetEventSlim.Reset();
        progressPercentage = null;
        analysisId = Guid.NewGuid();
    }

    private void ConcludeAnalysis()
    {
        stopwatch.Stop();
        State = DiskAnalysisState.Ready;
        manualResetEventSlim.Set();

        OnFinished();
    }

    private Task<DataSize> CalculateTotalSize()
    {
        return Task.Run(() =>
        {
            long dataSize = new DiskCrawler(RootPath, rootedBlackList)
                .AsParallel()
                .Where(x => x.Action == CrawlerAction.FileFound)
                .Select(x => new FileInfo(x.Path))
                .Sum(x => x.Length);

            return (DataSize)dataSize;
        });
    }

    private Task CalculateHashes()
    {
        return Task.Run(() =>
        {
            DiskCrawler diskCrawler = new(RootPath, rootedBlackList);

            foreach (CrawlerStep crawlerStep in diskCrawler)
            {
                switch (crawlerStep.Action)
                {
                    case CrawlerAction.DirectoryOpened:
                        if (crawlerStep.Path != RootPath)
                            AddDirectory(crawlerStep.Path);
                        break;

                    case CrawlerAction.DirectoryClosed:
                        if (crawlerStep.Path != RootPath)
                            CloseDirectory();
                        break;

                    case CrawlerAction.FileFound:
                        AddFile(crawlerStep.Path);
                        break;

                    case CrawlerAction.Error:
                        ProcessError(crawlerStep);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        });
    }

    private void AddDirectory(string directoryPath)
    {
        string directoryName = Path.GetFileName(directoryPath);
        HDirectory hDirectory = new(directoryName);

        SnapshotWriter?.AddAndOpen(hDirectory);
    }

    private void CloseDirectory()
    {
        SnapshotWriter?.CloseDirectory();
    }

    private void AddFile(string filePath)
    {
        HFile hFile = AnalyzeFile(filePath);
        SnapshotWriter?.Add(hFile);

        if (progressPercentage != null)
            UpdateProgress(hFile.Size);
    }

    private HFile AnalyzeFile(string filePath)
    {
        HFile hFile = new()
        {
            Name = Path.GetFileName(filePath)
        };

        try
        {
            FileInfo fileInfo = new(filePath);
            hFile.LastModifiedTime = fileInfo.LastWriteTimeUtc;

            using FileStream stream = File.OpenRead(filePath);

            hFile.Hash = md5.ComputeHash(stream);
            long size = stream.Length;

            hFile.Size = size;
        }
        catch (Exception ex)
        {
            OnErrorEncountered(new ErrorEncounteredEventArgs(ex, filePath));
            hFile.Error = ex.Message;
        }

        return hFile;
    }

    private void UpdateProgress(DataSize dataSize)
    {
        progressPercentage.UnderlyingValue += dataSize;

        DiskAnalysisProgressEventArgs args = new(progressPercentage);
        OnProgress(args);
    }

    private void ProcessError(CrawlerStep crawlerStep)
    {
        ErrorEncounteredEventArgs args = new(crawlerStep.Exception, crawlerStep.Path);
        OnErrorEncountered(args);

        HDirectory hDirectory = new()
        {
            Name = Path.GetFileName(crawlerStep.Path),
            Error = crawlerStep.Exception.Message
        };

        SnapshotWriter?.Add(hDirectory);
    }

    public void WaitToEnd()
    {
        manualResetEventSlim.Wait();
    }

    public void Dispose()
    {
        md5?.Dispose();
    }

    private void OnErrorEncountered(ErrorEncounteredEventArgs e)
    {
        ErrorEncountered?.Invoke(this, e);
    }

    private void OnStarting(DiskReaderStartingEventArgs e)
    {
        Starting?.Invoke(this, e);
    }

    private void OnProgress(DiskAnalysisProgressEventArgs e)
    {
        Progress?.Invoke(this, e);
    }

    private void OnFinished()
    {
        Finished?.Invoke(this, EventArgs.Empty);
    }
}