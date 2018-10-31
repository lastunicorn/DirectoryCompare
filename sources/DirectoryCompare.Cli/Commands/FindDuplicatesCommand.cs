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
using DustInTheWind.DirectoryCompare.Serialization;
using DustInTheWind.DirectoryCompare.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DustInTheWind.DirectoryCompare.Cli.Commands
{
    internal class FindDuplicatesCommand : ICommand
    {
        public ProjectLogger Logger { get; set; }
        public string Path1 { get; set; }
        public ConsoleDuplicatesExporter Exporter { get; set; }

        public void DisplayInfo()
        {
        }

        public void Execute()
        {
            JsonFileSerializer serializer = new JsonFileSerializer();
            XContainer xContainer1 = serializer.ReadFromFile(Path1);

            List<Tuple<string, XFile>> files = new List<Tuple<string, XFile>>();
            Read(files, xContainer1, Path.DirectorySeparatorChar.ToString());

            int duplicateCount = 0;

            for (int i = 0; i < files.Count; i++)
            {
                for (int j = i + 1; j < files.Count; j++)
                {
                    Tuple<string, XFile> tuple1 = files[i];
                    Tuple<string, XFile> tuple2 = files[j];

                    bool areEqual = ByteArrayCompare.AreEqual(tuple1.Item2.Hash, tuple2.Item2.Hash);

                    if (areEqual)
                    {
                        duplicateCount++;

                        string path1 = tuple1.Item1;
                        string path2 = tuple2.Item1;

                        string fullPath1 = Path.Combine(xContainer1.OriginalPath, path1.Substring(1));
                        string fullPath2 = Path.Combine(xContainer1.OriginalPath, path2.Substring(1));

                        long size = File.Exists(fullPath1)
                            ? new FileInfo(fullPath1).Length
                            : File.Exists(fullPath2)
                                ? new FileInfo(fullPath2).Length
                                : 0;

                        Exporter.WriteDuplicate(fullPath1, fullPath2, size);
                    }
                }
            }

            Exporter.WriteSummary(duplicateCount);
        }

        private void Read(List<Tuple<string, XFile>> files, XDirectory xDirectory, string parentPath)
        {
            if (xDirectory.Files != null)
                foreach (XFile xFile in xDirectory.Files)
                {
                    string filePath = Path.Combine(parentPath, xFile.Name);
                    files.Add(new Tuple<string, XFile>(filePath, xFile));
                }

            if (xDirectory.Directories != null)
                foreach (XDirectory xSubDirectory in xDirectory.Directories)
                {
                    string subdirectoryPath = Path.Combine(parentPath, xSubDirectory.Name);
                    Read(files, xSubDirectory, subdirectoryPath);
                }
        }
    }
}
