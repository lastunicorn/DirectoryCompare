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

namespace DustInTheWind.DirectoryCompare.InMemoryExport
{
    public class ContainerBuilder
    {
        private readonly Stack<XDirectory> directoryStack = new Stack<XDirectory>();

        public XContainer Container { get; }

        public ContainerBuilder()
        {
            Container = new XContainer
            {
                CreationTime = DateTime.UtcNow
            };
        }

        public ContainerBuilder(string path)
        {
            Container = new XContainer
            {
                CreationTime = DateTime.UtcNow,
                OriginalPath = path
            };
        }

        public void SetOriginalPath(string originalPath)
        {
            Container.OriginalPath = originalPath;
        }

        public void Add(XFile xFile)
        {
            if (directoryStack.Count == 0)
                throw new Exception("There is no directory added.");

            XDirectory topDirectory = directoryStack.Peek();
            topDirectory.Files.Add(xFile);
        }

        public void Add(XDirectory xDirectory)
        {
            if (directoryStack.Count == 0)
            {
                Container.Name = xDirectory.Name;
                Container.Files = xDirectory.Files;
                Container.Directories = xDirectory.Directories;
                Container.Error = xDirectory.Error;
            }
            else
            {
                XDirectory topDirectory = directoryStack.Peek();
                topDirectory.Directories.Add(xDirectory);
            }
        }

        public void AddAndOpen(XDirectory xDirectory)
        {
            if (directoryStack.Count == 0)
            {
                Container.Name = xDirectory.Name;
                Container.Files = xDirectory.Files;
                Container.Directories = xDirectory.Directories;
                Container.Error = xDirectory.Error;
            }
            else
            {
                XDirectory topDirectory = directoryStack.Peek();
                topDirectory.Directories.Add(xDirectory);
            }

            directoryStack.Push(xDirectory);
        }

        public void CloseDirectory()
        {
            directoryStack.Pop();
        }
    }
}