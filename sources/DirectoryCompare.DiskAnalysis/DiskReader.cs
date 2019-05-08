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
using DustInTheWind.DirectoryCompare.Common.Utils;
using DustInTheWind.DirectoryCompare.DiskAnalysis.DiskCrawling;
using DustInTheWind.DirectoryCompare.Entities;

namespace DustInTheWind.DirectoryCompare.DiskAnalysis
{
    public sealed class DiskReader : IDisposable
    {
        private readonly string rootPath;
        private readonly IDiskAnalysisExport diskAnalysisExport;
        private readonly MD5 md5;

        public PathCollection BlackList { get; } = new PathCollection();

        public event EventHandler<ErrorEncounteredEventArgs> ErrorEncountered;
        public event EventHandler<DiskReaderStartingEventArgs> Starting;

        public DiskReader(string rootPath, IDiskAnalysisExport diskAnalysisExport)
        {
            this.rootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
            this.diskAnalysisExport = diskAnalysisExport ?? throw new ArgumentNullException(nameof(diskAnalysisExport));

            md5 = MD5.Create();
        }

        public void Read()
        {
            PathCollection rootedBlackList = BlackList.ToAbsolutePaths(rootPath);

            OnStarting(new DiskReaderStartingEventArgs(rootedBlackList));

            diskAnalysisExport.Open(rootPath);

            DiskCrawler diskCrawler = new DiskCrawler(rootPath, rootedBlackList);

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

            diskAnalysisExport.Close();
        }

        private void AddDirectory(CrawlerStep crawlerStep)
        {
            string directoryName = Path.GetFileName(crawlerStep.Path);
            HDirectory hDirectory = new HDirectory(directoryName);

            diskAnalysisExport.OpenNewDirectory(hDirectory);
        }

        private void CloseDirectory()
        {
            diskAnalysisExport.CloseDirectory();
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

            diskAnalysisExport.Add(hFile);
        }

        private void ProcessError(CrawlerStep crawlerStep)
        {
            OnErrorEncountered(new ErrorEncounteredEventArgs(crawlerStep.Exception, crawlerStep.Path));

            HDirectory hDirectory = new HDirectory
            {
                Name = Path.GetFileName(crawlerStep.Path),
                Error = crawlerStep.Exception.Message
            };

            diskAnalysisExport.Add(hDirectory);
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