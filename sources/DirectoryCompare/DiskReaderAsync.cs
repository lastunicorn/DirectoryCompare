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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace DustInTheWind.DirectoryCompare
{
    public class DiskReaderAsync : IContainerProvider
    {
        private readonly string rootPath;

        public Container Container { get; private set; }
        private readonly ConcurrentQueue<Task<HashResult>> tasks = new ConcurrentQueue<Task<HashResult>>();

        public DiskReaderAsync(string rootPath)
        {
            this.rootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
        }

        public void Read()
        {
            Container = new Container
            {
                OriginalPath = rootPath
            };

            if (Directory.Exists(rootPath))
            {
                Task<HashResult> task = Task.Run(() => new HashResult
                {
                    XDirectory = Container,
                    XSubdirectory = Container
                });

                IEnumerable<Task<HashResult>> items = ReadDirectory1(task, rootPath);

                bool isFinished = false;

                Task creatorTask = Task.Run(() =>
                {
                    foreach (Task<HashResult> item in items)
                    {
                        while (tasks.Count >= 100)
                            Thread.Sleep(500);

                        tasks.Enqueue(item);
                    }

                    isFinished = true;
                });

                Task consumerTask = Task.Run(() =>
                {
                    while (true)
                    {
                        while (tasks.TryDequeue(out Task<HashResult> item))
                        {
                            item.Wait();

                            if (item.Result.XSubdirectory != null)
                                item.Result.XDirectory.Directories.Add(item.Result.XSubdirectory);

                            if (item.Result.XFile != null)
                                item.Result.XDirectory.Files.Add(item.Result.XFile);
                        }

                        if (isFinished)
                            break;

                        Thread.Sleep(500);
                    }
                });

                Task.WaitAll(creatorTask, consumerTask);
            }
        }

        private struct HashResult
        {
            public XDirectory XDirectory { get; set; }
            public XFile XFile { get; set; }
            public XDirectory XSubdirectory { get; set; }
        }

        private IEnumerable<Task<HashResult>> ReadDirectory1(Task<HashResult> xDirectory, string path)
        {
            string[] filePaths = Directory.GetFiles(path);

            foreach (string filePath in filePaths)
                yield return CalulateFileHash(xDirectory, filePath);

            string[] directoryPaths = Directory.GetDirectories(path);

            foreach (string directoryPath in directoryPaths)
            {
                Task<HashResult> processSubdirectoryTask = ProcessSubdirectory(xDirectory, directoryPath);
                yield return processSubdirectoryTask;

                IEnumerable<Task<HashResult>> items = ReadDirectory1(processSubdirectoryTask, directoryPath);

                foreach (Task<HashResult> item in items)
                    yield return item;
            }
        }

        private Task<HashResult> CalulateFileHash(Task<HashResult> xDirectory, string filePath)
        {
            return Task.Run(() =>
            {
                string fileName = Path.GetFileName(filePath);

                using (FileStream stream = File.OpenRead(filePath))
                {
                    using (MD5 md5 = MD5.Create())
                    {
                        byte[] hash = md5.ComputeHash(stream);

                        return new HashResult
                        {
                            XDirectory = xDirectory.Result.XSubdirectory,
                            XFile = new XFile { Name = fileName, Hash = hash }
                        };
                    }
                }
            });
        }

        private Task<HashResult> ProcessSubdirectory(Task<HashResult> xDirectory, string directoryPath)
        {
            return Task.Run(() =>
            {
                string directoryName = Path.GetFileName(directoryPath);

                return new HashResult
                {
                    XDirectory = xDirectory.Result.XSubdirectory,
                    XSubdirectory = new XDirectory { Name = directoryName }
                };
            });
        }
    }
}