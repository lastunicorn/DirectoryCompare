// DirectoryCompare
// Copyright (C) 2017 Dust in the Wind
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

using DustInTheWind.DirectoryCompare.DiskCrawling;
using System;
using System.IO;
using System.Security.Cryptography;

namespace DustInTheWind.DirectoryCompare
{
    public sealed class DiskReader : IContainerProvider, IDisposable
    {
        private readonly string rootPath;
        private readonly MD5 md5;
        private DirectoryStack directoryStack;

        public XContainer Container { get; private set; }
        public PathCollection BlackList { get; } = new PathCollection();

        public event EventHandler<ErrorEncounteredEventArgs> ErrorEncountered;
        public event EventHandler<DiskReaderStartingEventArgs> Starting;

        public DiskReader(string rootPath)
        {
            this.rootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));

            md5 = MD5.Create();
        }

        public void Read()
        {
            PathCollection rootedBlackList = BlackList.ToAbsolutePaths(rootPath);

            OnStarting(new DiskReaderStartingEventArgs(rootedBlackList));
            
            directoryStack = new DirectoryStack();

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

            CreateContainer();
        }

        private void AddDirectory(CrawlerStep crawlerStep)
        {
            string directoryName = Path.GetFileName(crawlerStep.Path);
            XDirectory xDirectory = new XDirectory(directoryName);

            if (directoryStack.HasDirectory)
                directoryStack.AddToCurrentDirectory(xDirectory);

            directoryStack.Add(xDirectory);
        }

        private void CloseDirectory()
        {
            directoryStack.RemoveLast();
        }

        private void AddFile(CrawlerStep crawlerStep)
        {
            XFile xFile = new XFile
            {
                Name = Path.GetFileName(crawlerStep.Path)
            };

            try
            {
                using (FileStream stream = File.OpenRead(crawlerStep.Path))
                {
                    xFile.Hash = md5.ComputeHash(stream);
                }
            }
            catch (Exception ex)
            {
                OnErrorEncountered(new ErrorEncounteredEventArgs(ex, crawlerStep.Path));
                xFile.Error = ex.Message;
            }

            directoryStack.AddToCurrentDirectory(xFile);
        }

        private void ProcessError(CrawlerStep crawlerStep)
        {
            OnErrorEncountered(new ErrorEncounteredEventArgs(crawlerStep.Exception, crawlerStep.Path));

            XDirectory xDirectory = new XDirectory
            {
                Name = Path.GetFileName(crawlerStep.Path),
                Error = crawlerStep.Exception.Message
            };

            directoryStack.AddToCurrentDirectory(xDirectory);
        }

        private void CreateContainer()
        {
            XDirectory lastDirectory = directoryStack.LastRemoved;

            Container = new XContainer
            {
                OriginalPath = rootPath,
                CreationTime = DateTime.UtcNow,
                Name = lastDirectory.Name,
                Files = lastDirectory.Files,
                Directories = lastDirectory.Directories,
                Error = lastDirectory.Error
            };
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