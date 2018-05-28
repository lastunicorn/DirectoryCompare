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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace DustInTheWind.DirectoryCompare
{
    public class DiskReader : IContainerProvider, IDisposable
    {
        private readonly string rootPath;
        private readonly MD5 md5;

        public Container Container { get; private set; }
        public List<string> BlackList { get; set; }

        private List<string> computedBlackList = new List<string>();

        public event EventHandler<ErrorEncounteredEventArgs> ErrorEncountered;

        public DiskReader(string rootPath)
        {
            this.rootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));

            md5 = MD5.Create();
        }

        public void Read()
        {
            if (BlackList == null)
                computedBlackList = new List<string>();
            else
                computedBlackList = BlackList
                    .Select(x => Path.IsPathRooted(x) ? x : Path.Combine(rootPath, x))
                    .ToList();

            Console.WriteLine("Computed black list:");

            foreach (string blackListItem in computedBlackList)
                Console.WriteLine("- " + blackListItem);

            Container = new Container
            {
                OriginalPath = rootPath,
                CreationTime = DateTime.UtcNow
            };

            try
            {
                if (!Directory.Exists(rootPath))
                    throw new Exception(string.Format("The path '{0}' does not exist.", rootPath));

                ReadDirectory(Container, rootPath);
            }
            catch (Exception ex)
            {
                OnErrorEncountered(new ErrorEncounteredEventArgs(ex, rootPath));

                Container.Error = ex.Message;
            }
        }

        private void ReadDirectory(XDirectory xDirectory, string path)
        {
            string[] filePaths = Directory.GetFiles(path)
                .Where(x => !computedBlackList.Contains(x))
                .ToArray();

            if (filePaths.Length > 0)
            {
                xDirectory.Files = new List<XFile>(filePaths.Length);

                foreach (string filePath in filePaths)
                {
                    XFile xFile = ProcessFile(filePath);
                    xDirectory.Files.Add(xFile);
                }
            }

            string[] directoryPaths = Directory.GetDirectories(path)
                .Where(x => !computedBlackList.Contains(x))
                .ToArray();

            if (directoryPaths.Length > 0)
            {
                xDirectory.Directories = new List<XDirectory>(directoryPaths.Length);

                foreach (string directoryPath in directoryPaths)
                {
                    XDirectory xSubdirectory = ProcessDirectory(directoryPath);
                    xDirectory.Directories.Add(xSubdirectory);
                }
            }
        }

        private XDirectory ProcessDirectory(string directoryPath)
        {
            string directoryName = Path.GetFileName(directoryPath);

            try
            {
                Console.WriteLine(directoryPath);

                XDirectory xDirectory = new XDirectory
                {
                    Name = directoryName
                };

                ReadDirectory(xDirectory, directoryPath);
                return xDirectory;
            }
            catch (Exception ex)
            {
                OnErrorEncountered(new ErrorEncounteredEventArgs(ex, directoryPath));

                return new XDirectory
                {
                    Name = directoryName,
                    Error = ex.Message
                };
            }
        }

        private XFile ProcessFile(string filePath)
        {
            try
            {
                using (FileStream stream = File.OpenRead(filePath))
                {
                    return new XFile
                    {
                        Name = Path.GetFileName(filePath),
                        Hash = md5.ComputeHash(stream)
                    };
                }
            }
            catch (Exception ex)
            {
                OnErrorEncountered(new ErrorEncounteredEventArgs(ex, filePath));

                return new XFile
                {
                    Name = Path.GetFileName(filePath),
                    Error = ex.Message
                };
            }
        }

        public void Dispose()
        {
            md5?.Dispose();
        }

        protected virtual void OnErrorEncountered(ErrorEncounteredEventArgs e)
        {
            ErrorEncountered?.Invoke(this, e);
        }
    }
}