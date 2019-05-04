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

namespace DustInTheWind.DirectoryCompare
{
    internal class DirectoryStack
    {
        private readonly Stack<XDirectory> directoryStack = new Stack<XDirectory>();

        public bool HasDirectory => directoryStack.Count > 0;
        public XDirectory LastRemoved { get; private set; }

        public void AddToCurrentDirectory(XFile xFile)
        {
            if (directoryStack.Count == 0)
                throw new Exception("There is no directory in the stack.");

            XDirectory currentDirectory = directoryStack.Peek();
            currentDirectory.Files.Add(xFile);
        }

        public void AddToCurrentDirectory(XDirectory xDirectory)
        {
            if (directoryStack.Count == 0)
                throw new Exception("There is no directory in the stack.");

            XDirectory currentDirectory = directoryStack.Peek();
            currentDirectory.Directories.Add(xDirectory);
        }

        public void Add(XDirectory xDirectory)
        {
            directoryStack.Push(xDirectory);
        }

        public void RemoveLast()
        {
            LastRemoved = directoryStack.Pop();
        }
    }
}