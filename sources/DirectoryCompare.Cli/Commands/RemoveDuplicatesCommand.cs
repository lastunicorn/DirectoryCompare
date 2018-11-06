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

using DustInTheWind.DirectoryCompare.Cli.ResultExporters;
using System.Collections.Generic;
using System.IO;

namespace DustInTheWind.DirectoryCompare.Cli.Commands
{
    internal class RemoveDuplicatesCommand : ICommand
    {
        public ProjectLogger Logger { get; set; }
        public string PathLeft { get; set; }
        public string PathRight { get; set; }
        public ConsoleRemoveDuplicatesExporter Exporter { get; set; }
        public FileRemove FileRemove { get; set; }

        public void DisplayInfo()
        {
        }

        public void Execute()
        {
            DuplicatesProvider duplicatesProvider = new DuplicatesProvider
            {
                PathLeft = PathLeft,
                PathRight = PathRight,
                CheckFilesExist = true
            };

            IEnumerable<Duplicate> duplicates = duplicatesProvider.Find();

            int removeCount = 0;
            long totalSize = 0;

            foreach (Duplicate duplicate in duplicates)
            {
                if (!duplicate.AreEqual)
                    continue;

                bool file1Exists = duplicate.File1Exists;
                bool file2Exists = duplicate.File2Exists;

                if (file1Exists && file2Exists)
                {
                    switch (FileRemove)
                    {
                        case FileRemove.Left:
                            File.Delete(duplicate.FullPath1);
                            removeCount++;
                            totalSize += duplicate.Size;
                            Exporter.WriteRemove(duplicate.FullPath1);
                            break;

                        case FileRemove.Right:
                            File.Delete(duplicate.FullPath2);
                            removeCount++;
                            totalSize += duplicate.Size;
                            Exporter.WriteRemove(duplicate.FullPath2);
                            break;
                    }
                }
            }

            Exporter.WriteSummary(removeCount, totalSize);
        }
    }
}
