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

using System;
using System.IO;
using System.Security.Cryptography;
using DustInTheWind.DirectoryCompare.DiskAnalysis.DiskCrawling;
using DustInTheWind.DirectoryCompare.Entities;
using DustInTheWind.DirectoryCompare.Utils;

namespace DustInTheWind.DirectoryCompare.DiskAnalysis
{
    public sealed class DiskAnalyzer : IDiskAnalyzer, IDisposable
    {
        private readonly MD5 md5;
        private string rootPath;

        public string RootPath
        {
            get => rootPath;
            set => rootPath = value ?? string.Empty;
        }

        public IAnalysisExport AnalysisExport { get; set; }

        public PathCollection BlackList { get; set; }

        public event EventHandler<ErrorEncounteredEventArgs> ErrorEncountered;
        public event EventHandler<DiskReaderStartingEventArgs> Starting;

        public DiskAnalyzer()
        {
            md5 = MD5.Create();
        }

        public void Run()
        {
            PathCollection rootedBlackList = BlackList?.PrependPath(RootPath) ?? new PathCollection();

            OnStarting(new DiskReaderStartingEventArgs(rootedBlackList));

            AnalysisExport?.Open(RootPath);

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

            AnalysisExport?.Close();
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
                    hFile.Hash = md5.ComputeHash(stream);
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