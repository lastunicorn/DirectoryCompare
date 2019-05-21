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
using System.Collections.Generic;
using DustInTheWind.DirectoryCompare.Entities;

namespace DustInTheWind.DirectoryCompare.DiskAnalysis
{
    public class SnapshotDiskAnalysisExport : IDiskAnalysisExport
    {
        private readonly Stack<HDirectory> directoryStack = new Stack<HDirectory>();

        public Snapshot Snapshot { get; }

        public SnapshotDiskAnalysisExport()
        {
            Snapshot = new Snapshot
            {
                CreationTime = DateTime.UtcNow
            };
        }

        public void Open(string originalPath)
        {
            Snapshot.OriginalPath = originalPath;
        }

        public void AddAndOpen(HDirectory directory)
        {
            Add(directory);
            directoryStack.Push(directory);
        }

        public void CloseDirectory()
        {
            directoryStack.Pop();
        }

        public void Add(HFile file)
        {
            if (directoryStack.Count == 0)
                throw new Exception("There is no directory added that can be used as parent for this file.");

            HDirectory topDirectory = directoryStack.Peek();
            topDirectory.Files.Add(file);
        }

        public void Add(HDirectory directory)
        {
            if (directoryStack.Count == 0)
            {
                Snapshot.Name = directory.Name;
                Snapshot.Files.AddRange(directory.Files);
                Snapshot.Directories.AddRange(directory.Directories);
                Snapshot.Error = directory.Error;
            }
            else
            {
                HDirectory topDirectory = directoryStack.Peek();
                topDirectory.Directories.Add(directory);
            }
        }

        public void Close()
        {
        }
    }
}