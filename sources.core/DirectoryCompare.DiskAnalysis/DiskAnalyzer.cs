// DirectoryCompare
// Copyright (C) 2017-2019 Dust in the Wind
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

using DustInTheWind.DirectoryCompare.DiskAnalysis.DiskCrawling;
using DustInTheWind.DirectoryCompare.Domain.DiskAnalysis;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Domain.Utils;
using System;
using System.IO;
using System.Security.Cryptography;

namespace DustInTheWind.DirectoryCompare.DiskAnalysis
{
    public sealed class DiskAnalyzer : IDiskAnalyzer, IDisposable
    {
        private readonly MD5 md5;
        private string rootPath;
        private long totalSize;
        private long readSize;

        public string RootPath
        {
            get => rootPath;
            set => rootPath = value ?? string.Empty;
        }

        public IAnalysisExport AnalysisExport { get; set; }

        public PathCollection BlackList { get; set; }

        public event EventHandler<ErrorEncounteredEventArgs> ErrorEncountered;
        public event EventHandler<DiskReaderStartingEventArgs> Starting;

        public IProgress<float> ProgressIndicator { get; set; }

        public DiskAnalyzer()
        {
            md5 = MD5.Create();
        }

        public void Run()
        {
            PathCollection rootedBlackList = BlackList?.PrependPath(RootPath) ?? new PathCollection();

            OnStarting(new DiskReaderStartingEventArgs(rootedBlackList));

            AnalysisExport?.Open(RootPath);

            totalSize = CalculateSize(rootedBlackList);
            CalculateHashes(rootedBlackList);

            AnalysisExport?.Close();
        }

        private long CalculateSize(PathCollection rootedBlackList)
        {
            DiskCrawler diskCrawler = new DiskCrawler(RootPath, rootedBlackList);
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
                                FileInfo fileInfo = new FileInfo(crawlerStep.Path);
                                size += fileInfo.Length;
                            }
                            catch { }
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

        private void CalculateHashes(PathCollection rootedBlackList)
        {
            DiskCrawler diskCrawler = new DiskCrawler(RootPath, rootedBlackList);

            foreach (CrawlerStep crawlerStep in diskCrawler)
            {
                switch (crawlerStep.Action)
                {
                    case CrawlerAction.DirectoryOpened:
                        AddDirectory(crawlerStep);
                        break;

                    case CrawlerAction.DirectoryClosed:
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
            HDirectory hDirectory = new HDirectory(directoryName);

            AnalysisExport?.AddAndOpen(hDirectory);
        }

        private void CloseDirectory()
        {
            AnalysisExport?.CloseDirectory();
        }

        private void AddFile(CrawlerStep crawlerStep)
        {
            HFile hFile = new HFile
            {
                Name = Path.GetFileName(crawlerStep.Path)
            };

            try
            {
                using (FileStream stream = File.OpenRead(crawlerStep.Path))
                {
                    hFile.Hash = md5.ComputeHash(stream);
                    readSize += stream.Length;

                    if (totalSize > 0)
                        ProgressIndicator?.Report(readSize * 100 / totalSize);
                }
            }
            catch (Exception ex)
            {
                OnErrorEncountered(new ErrorEncounteredEventArgs(ex, crawlerStep.Path));
                hFile.Error = ex.Message;
            }

            AnalysisExport?.Add(hFile);
        }

        private void ProcessError(CrawlerStep crawlerStep)
        {
            OnErrorEncountered(new ErrorEncounteredEventArgs(crawlerStep.Exception, crawlerStep.Path));

            HDirectory hDirectory = new HDirectory
            {
                Name = Path.GetFileName(crawlerStep.Path),
                Error = crawlerStep.Exception.Message
            };

            AnalysisExport?.Add(hDirectory);
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
    }
}