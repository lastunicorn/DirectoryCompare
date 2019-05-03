﻿// DirectoryCompare
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
using System;
using System.Collections.Generic;
using System.IO;

namespace DustInTheWind.DirectoryCompare.Cli.Commands
{

    internal class FindDuplicatesCommand : ICommand
    {
        public ProjectLogger Logger { get; set; }
        public string PathLeft { get; set; }
        public string PathRight { get; set; }
        public ConsoleDuplicatesExporter Exporter { get; set; }
        public bool CheckFilesExist { get; set; }

        public void DisplayInfo()
        {
        }

        public void Initialize(Arguments arguments)
        {
            Logger = new ProjectLogger();

            if (arguments.Count == 0)
                throw new Exception("Invalid command parameters.");

            PathLeft = arguments[0];

            if (arguments.Count > 1)
            {
                bool isFileRight = File.Exists(arguments[1]);

                if (isFileRight)
                {
                    PathRight = arguments[1];

                    if (arguments.Count > 2)
                        CheckFilesExist = bool.Parse(arguments[2]);
                }
                else
                {
                    PathRight = null;
                    CheckFilesExist = bool.Parse(arguments[1]);
                }
            }
            else
            {
                PathRight = null;
                CheckFilesExist = false;
            }

            Exporter = new ConsoleDuplicatesExporter();
        }

        public void Execute()
        {
            DuplicatesProvider duplicatesProvider = new DuplicatesProvider
            {
                PathLeft = PathLeft,
                PathRight = PathRight,
                CheckFilesExist = CheckFilesExist
            };

            IEnumerable<Duplicate> duplicates = duplicatesProvider.Find();

            int duplicateCount = 0;
            long totalSize = 0;

            foreach (Duplicate duplicate in duplicates)
            {
                if (duplicate.AreEqual)
                {
                    duplicateCount++;
                    totalSize += duplicate.Size;
                    Exporter.WriteDuplicate(duplicate.FullPath1, duplicate.FullPath2, duplicate.Size);
                }
            }

            Exporter.WriteSummary(duplicateCount, totalSize);
        }
    }
}