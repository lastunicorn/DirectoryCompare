using System;
using System.IO;
using System.Security.Cryptography;

namespace DirectoryCompare
{
    internal class DiskReader : IContainerProvider, IDisposable
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