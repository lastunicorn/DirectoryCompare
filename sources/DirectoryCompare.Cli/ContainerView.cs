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

namespace DustInTheWind.DirectoryCompare.Cli
{
    internal class ContainerView
    {
        private readonly XContainer xContainer;

        public ContainerView(XContainer xContainer)
        {
            this.xContainer = xContainer;
        }

        public void Display()
        {
            DisplayDirectory(xContainer, 0);
        }

        private void DisplayDirectory(XDirectory xDirectory, int index)
        {
            string indent = new string(' ', index);

            foreach (XDirectory xSubdirectory in xDirectory.Directories)
            {
                Console.WriteLine(indent + xSubdirectory.Name);
                DisplayDirectory(xSubdirectory, index + 1);
            }

            foreach (XFile xFile in xDirectory.Files)
                Console.WriteLine(indent + xFile.Name);
        }
    }
}