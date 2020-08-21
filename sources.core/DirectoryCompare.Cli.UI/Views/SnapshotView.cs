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
using DustInTheWind.ConsoleTools;
using DustInTheWind.DirectoryCompare.Domain.Entities;

namespace DustInTheWind.DirectoryCompare.Cli.UI.Views
{
    internal class SnapshotView
    {
        private readonly Snapshot snapshot;

        public SnapshotView(Snapshot snapshot)
        {
            this.snapshot = snapshot;
        }

        public void Display()
        {
            DisplayDirectory(snapshot, 0);
        }

        private static void DisplayDirectory(HDirectory hDirectory, int index)
        {
            string indent = new string(' ', index);

            foreach (HDirectory xSubdirectory in hDirectory.Directories)
            {
                Console.WriteLine(indent + xSubdirectory.Name);
                DisplayDirectory(xSubdirectory, index + 1);
            }

            foreach (HFile xFile in hDirectory.Files)
            {
                Console.Write(indent + xFile.Name);
                CustomConsole.WriteLine(ConsoleColor.DarkGray, " [" + xFile.Hash + "]");
            }
        }
    }
}