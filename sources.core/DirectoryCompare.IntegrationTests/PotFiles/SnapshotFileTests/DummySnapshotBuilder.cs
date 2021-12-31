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
using System.Linq;
using DustInTheWind.DirectoryCompare.Domain.Entities;

namespace DustInTheWind.DirectoryCompare.IntegrationTests.PotFiles.SnapshotFileTests
{
    internal class DummySnapshotBuilder
    {
        private readonly Random random = new();

        private int randomFileCount;
        private int randomDirectoryCount;

        public DummySnapshotBuilder AddFiles(int count)
        {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            randomFileCount += count;

            return this;
        }

        public DummySnapshotBuilder AddDirectories(int count)
        {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            randomDirectoryCount += count;

            return this;
        }

        public Snapshot Build()
        {
            Snapshot snapshot = new()
            {
                OriginalPath = "/media/alez/SomePath",
                CreationTime = new DateTime(random.Next())
            };

            for (int i = 0; i < randomFileCount; i++)
            {
                HFile file = CreateRandomFile();
                snapshot.Files.Add(file);
            }

            for (int i = 0; i < randomDirectoryCount; i++)
            {
                HDirectory directory = CreateRandomDirectory();
                snapshot.Directories.Add(directory);
            }

            return snapshot;
        }

        private HFile CreateRandomFile()
        {
            return new HFile
            {
                Name = "file-" + random.Next(),
                Size = random.Next(),
                LastModifiedTime = new DateTime(random.Next()),
                Hash = CreateRandomHash()
            };
        }

        private byte[] CreateRandomHash()
        {
            return Enumerable.Range(0, 31)
                .Select(x => (byte)random.Next(256))
                .ToArray();
        }

        private HDirectory CreateRandomDirectory()
        {
            HDirectory directory = new()
            {
                Name = "directory-" + random.Next()
            };

            for (int i = 0; i < 10; i++)
            {
                HFile file = CreateRandomFile();
                directory.Files.Add(file);
            }
            
            return directory;
        }
    }
}