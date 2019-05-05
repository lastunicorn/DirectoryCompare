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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using DustInTheWind.DirectoryCompare.Entities;

namespace DustInTheWind.DirectoryCompare.DiskAnalysing
{
    public class DiskReaderAsync : IContainerProvider
    {
        private readonly string rootPath;

        public HContainer Container { get; private set; }
        private readonly ConcurrentQueue<Task<HashResult>> tasks = new ConcurrentQueue<Task<HashResult>>();

        public DiskReaderAsync(string rootPath)
        {
            this.rootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
        }

        public void Read()
        {
            Container = new HContainer
            {
                OriginalPath = rootPath,
                CreationTime = DateTime.UtcNow
            };

            if (!Directory.Exists(rootPath))
                return;

            Task<HashResult> task = Task.Run(() => new HashResult
            {
                HDirectory = Container,
                HSubdirectory = Container
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

                        if (item.Result.HSubdirectory != null)
                            item.Result.HDirectory.Directories.Add(item.Result.HSubdirectory);

                        if (item.Result.HFile != null)
                            item.Result.HDirectory.Files.Add(item.Result.HFile);
                    }

                    if (isFinished)
                        break;

                    Thread.Sleep(500);
                }
            });

            Task.WaitAll(creatorTask, consumerTask);
        }

        private struct HashResult
        {
            public HDirectory HDirectory { get; set; }
            public HFile HFile { get; set; }
            public HDirectory HSubdirectory { get; set; }
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

        private static Task<HashResult> CalulateFileHash(Task<HashResult> xDirectory, string filePath)
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
                            HDirectory = xDirectory.Result.HSubdirectory,
                            HFile = new HFile { Name = fileName, Hash = hash }
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
                    HDirectory = xDirectory.Result.HSubdirectory,
                    HSubdirectory = new HDirectory { Name = directoryName }
                };
            });
        }
    }
}