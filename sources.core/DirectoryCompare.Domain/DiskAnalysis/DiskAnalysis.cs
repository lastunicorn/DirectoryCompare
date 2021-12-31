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
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using DustInTheWind.DirectoryCompare.Domain.DiskAnalysis.DiskCrawling;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Domain.ImportExport;
using DustInTheWind.DirectoryCompare.Domain.Utils;

namespace DustInTheWind.DirectoryCompare.Domain.DiskAnalysis
{
    public sealed class DiskAnalysis : IDiskAnalysisProgress, IDisposable
    {
        private readonly Stopwatch stopwatch = new();
        private readonly ManualResetEventSlim manualResetEventSlim = new(false);
        private readonly MD5 md5;
        private string rootPath;
        private long totalSize;
        private long readSize;

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

        public TimeSpan ElapsedTime => stopwatch.Elapsed;

        public DiskAnalysis()
        {
            md5 = MD5.Create();
        }

        public void Run()
        {
            if (State == DiskAnalysisState.InProgress)
                throw new DirectoryCompareException("Another analysis is still in progress.");

            State = DiskAnalysisState.InProgress;
            stopwatch.Start();
            manualResetEventSlim.Reset();

            try
            {
                DiskPathCollection rootedBlackList = BlackList?.PrependPath(RootPath) ?? new DiskPathCollection();

                OnStarting(new DiskReaderStartingEventArgs(rootedBlackList));

                SnapshotWriter?.Open(RootPath);

                totalSize = CalculateSize(rootedBlackList);
                CalculateHashes(rootedBlackList);

                SnapshotWriter?.Close();
            }
            finally
            {
                stopwatch.Stop();
                State = DiskAnalysisState.Ready;
                manualResetEventSlim.Set();
            }
        }

        private long CalculateSize(DiskPathCollection rootedBlackList)
        {
            DiskCrawler diskCrawler = new(RootPath, rootedBlackList);
            long size = 0;

            foreach (CrawlerStep crawlerStep in diskCrawler)
            {
                switch (crawlerStep.Action)
                {
                    case CrawlerAction.DirectoryOpened:
                        break;

                    case CrawlerAction.DirectoryClosed:
                        break;

                    case CrawlerAction.FileFound:
                        {
                            try
                            {
                                FileInfo fileInfo = new(crawlerStep.Path);
                                size += fileInfo.Length;
                            }
                            catch
                            {
                            }
                        }
                        break;

                    case CrawlerAction.Error:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return size;
        }

        private void CalculateHashes(DiskPathCollection rootedBlackList)
        {
            DiskCrawler diskCrawler = new(RootPath, rootedBlackList);

            foreach (CrawlerStep crawlerStep in diskCrawler)
            {
                switch (crawlerStep.Action)
                {
                    case CrawlerAction.DirectoryOpened:
                        if (crawlerStep.Path != RootPath)
                            AddDirectory(crawlerStep);
                        break;

                    case CrawlerAction.DirectoryClosed:
                        if (crawlerStep.Path != RootPath)
                            CloseDirectory();
                        break;

                    case CrawlerAction.FileFound:
                        AddFile(crawlerStep);
                        break;

                    case CrawlerAction.Error:
                        ProcessError(crawlerStep);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void AddDirectory(CrawlerStep crawlerStep)
        {
            string directoryName = Path.GetFileName(crawlerStep.Path);
            HDirectory hDirectory = new(directoryName);

            SnapshotWriter?.AddAndOpen(hDirectory);
        }

        private void CloseDirectory()
        {
            SnapshotWriter?.CloseDirectory();
        }

        private void AddFile(CrawlerStep crawlerStep)
        {
            HFile hFile = new()
            {
                Name = Path.GetFileName(crawlerStep.Path)
            };

            try
            {
                FileInfo fileInfo = new(crawlerStep.Path);
                hFile.LastModifiedTime = fileInfo.LastWriteTimeUtc;

                using (FileStream stream = File.OpenRead(crawlerStep.Path))
                {
                    hFile.Hash = md5.ComputeHash(stream);
                    long size = stream.Length;

                    hFile.Size = size;
                    readSize += size;
                }

                if (totalSize > 0)
                {
                    long percentage = readSize * 100 / totalSize;
                    OnProgress(new DiskAnalysisProgressEventArgs(percentage));
                }
            }
            catch (Exception ex)
            {
                OnErrorEncountered(new ErrorEncounteredEventArgs(ex, crawlerStep.Path));
                hFile.Error = ex.Message;
            }

            SnapshotWriter?.Add(hFile);
        }

        private void ProcessError(CrawlerStep crawlerStep)
        {
            OnErrorEncountered(new ErrorEncounteredEventArgs(crawlerStep.Exception, crawlerStep.Path));

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
    }
}