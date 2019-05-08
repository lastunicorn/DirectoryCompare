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
    public class ContainerBuilder
    {
        private readonly Stack<HDirectory> directoryStack = new Stack<HDirectory>();

        public HContainer Container { get; }

        public ContainerBuilder()
        {
            Container = new HContainer
            {
                CreationTime = DateTime.UtcNow
            };
        }

        public ContainerBuilder(string path)
        {
            Container = new HContainer
            {
                CreationTime = DateTime.UtcNow,
                OriginalPath = path
            };
        }

        public void SetOriginalPath(string originalPath)
        {
            Container.OriginalPath = originalPath;
        }

        public void Add(HFile hFile)
        {
            if (directoryStack.Count == 0)
                throw new Exception("There is no directory added.");

            HDirectory topDirectory = directoryStack.Peek();
            topDirectory.Files.Add(hFile);
        }

        public void Add(HDirectory hDirectory)
        {
            if (directoryStack.Count == 0)
            {
                Container.Name = hDirectory.Name;
                Container.Files = hDirectory.Files;
                Container.Directories = hDirectory.Directories;
                Container.Error = hDirectory.Error;
            }
            else
            {
                HDirectory topDirectory = directoryStack.Peek();
                topDirectory.Directories.Add(hDirectory);
            }
        }

        public void AddAndOpen(HDirectory hDirectory)
        {
            if (directoryStack.Count == 0)
            {
                Container.Name = hDirectory.Name;
                Container.Files = hDirectory.Files;
                Container.Directories = hDirectory.Directories;
                Container.Error = hDirectory.Error;
            }
            else
            {
                HDirectory topDirectory = directoryStack.Peek();
                topDirectory.Directories.Add(hDirectory);
            }

            directoryStack.Push(hDirectory);
        }

        public void CloseDirectory()
        {
            directoryStack.Pop();
        }
    }
}