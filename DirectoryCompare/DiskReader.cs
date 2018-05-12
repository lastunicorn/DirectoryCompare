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
using System.IO;
using System.Security.Cryptography;

namespace DirectoryCompare
{
    public class DiskReader : IContainerProvider, IDisposable
    {
        private readonly string rootPath;
        private readonly MD5 md5;

        public Container Container { get; private set; }

        public DiskReader(string rootPath)
        {
            this.rootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));

            md5 = MD5.Create();
        }

        public void Read()
        {
            Container = new Container();

            if (Directory.Exists(rootPath))
                ReadDirectory(Container, rootPath);
        }

        private void ReadDirectory(XDirectory xDirectory, string path)
        {
            string[] filePaths = Directory.GetFiles(path);

            foreach (string filePath in filePaths)
            {
                string fileName = Path.GetFileName(filePath);

                using (FileStream stream = File.OpenRead(filePath))
                {
                    byte[] hash = md5.ComputeHash(stream);

                    xDirectory.Files.Add(new XFile { Name = fileName, Hash = hash });
                }
            }

            string[] directoryPaths = Directory.GetDirectories(path);

            foreach (string directoryPath in directoryPaths)
            {
                string directoryName = Path.GetFileName(directoryPath);
                XDirectory xSubdirectory = new XDirectory { Name = directoryName };
                xDirectory.Directories.Add(xSubdirectory);

                ReadDirectory(xSubdirectory, directoryPath);
            }
        }

        public void Dispose()
        {
            md5?.Dispose();
        }
    }
}