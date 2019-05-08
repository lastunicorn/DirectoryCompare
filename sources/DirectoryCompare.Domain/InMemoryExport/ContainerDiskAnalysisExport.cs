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

namespace DustInTheWind.DirectoryCompare.InMemoryExport
{
    public class ContainerDiskAnalysisExport : IDiskAnalysisExport
    {
        private readonly Stack<HDirectory> directoryStack = new Stack<HDirectory>();

        public HContainer Container { get; }

        public ContainerDiskAnalysisExport()
        {
            Container = new HContainer
            {
                CreationTime = DateTime.UtcNow
            };
        }

        public void Open(string originalPath)
        {
            Container.OriginalPath = originalPath;
        }

        public void OpenNewDirectory(HDirectory directory)
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
                throw new Exception("There is no directory added.");

            HDirectory topDirectory = directoryStack.Peek();
            topDirectory.Files.Add(file);
        }

        public void Add(HDirectory directory)
        {
            if (directoryStack.Count == 0)
            {
                Container.Name = directory.Name;
                Container.Files.AddRange(directory.Files);
                Container.Directories.AddRange(directory.Directories);
                Container.Error = directory.Error;
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