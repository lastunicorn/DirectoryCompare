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
using System.IO;
using System.Linq;
using DustInTheWind.DirectoryCompare.Cli.Commands;
using DustInTheWind.DirectoryCompare.Cli.ResultExporters;

namespace DustInTheWind.DirectoryCompare.Cli
{
    internal static class CommandBuilder
    {
        public static ICommand CreateCommand(Arguments arguments)
        {
            if (string.IsNullOrEmpty(arguments.Command))
                throw new Exception("Please provide a command name to execute.");

            switch (arguments.Command)
            {
                case "read-disk":
                    return new ReadDiskCommand
                    {
                        Logger = new ProjectLogger(),
                        SourcePath = arguments[0],
                        DestinationFilePath = arguments[1],
                        BlackList = ReadBlackList(arguments[2])
                    };

                case "read-file":
                    return new ReadFileCommand
                    {
                        Logger = new ProjectLogger(),
                        FilePath = arguments[0]
                    };

                case "verify-disk":
                    return new VerifyDiskCommand
                    {
                        Logger = new ProjectLogger(),
                        DiskPath = arguments[0],
                        FilePath = arguments[1],
                        Exporter = new ConsoleComparisonExporter()
                    };

                case "compare-disks":
                    return new CompareDisksCommand
                    {
                        Logger = new ProjectLogger(),
                        Path1 = arguments[0],
                        Path2 = arguments[1],
                        Exporter = new ConsoleComparisonExporter()
                    };

                case "compare-files":
                    return new CompareFilesCommand
                    {
                        Logger = new ProjectLogger(),
                        Path1 = arguments[0],
                        Path2 = arguments[1],
                        Exporter = arguments.Count >= 3
                            ? (IComparisonExporter)new FileComparisonExporter { ResultsDirectory = arguments[2] }
                            : (IComparisonExporter)new ConsoleComparisonExporter()
                    };

                case "find-duplicates":
                    {
                        string pathLeft = null;
                        string pathRight = null;
                        bool checkFilesExist = false;

                        if (arguments.Count == 0)
                            throw new Exception("Invalid command parameters.");

                        pathLeft = arguments[0];

                        if (arguments.Count > 1)
                        {
                            bool isFileRight = File.Exists(arguments[1]);

                            if (isFileRight)
                            {
                                pathRight = arguments[1];

                                if (arguments.Count > 2)
                                    checkFilesExist = bool.Parse(arguments[2]);
                            }
                            else
                            {
                                checkFilesExist = bool.Parse(arguments[1]);
                            }
                        }

                        return new FindDuplicatesCommand()
                        {
                            Logger = new ProjectLogger(),
                            PathLeft = pathLeft,
                            PathRight = pathRight,
                            Exporter = new ConsoleDuplicatesExporter(),
                            CheckFilesExist = checkFilesExist
                        };
                    }

                case "remove-duplicates":
                    {
                        string pathLeft = null;
                        string pathRight = null;
                        FileRemove fileRemove = FileRemove.Right;

                        if (arguments.Count == 0)
                            throw new Exception("Invalid command parameters.");

                        pathLeft = arguments[0];

                        if (arguments.Count > 1)
                        {
                            bool isFileRight = File.Exists(arguments[1]);

                            if (isFileRight)
                            {
                                pathRight = arguments[1];

                                if (arguments.Count > 2)
                                    fileRemove = (FileRemove)Enum.Parse(typeof(FileRemove), arguments[2]);
                            }
                            else
                            {
                                fileRemove = (FileRemove)Enum.Parse(typeof(FileRemove), arguments[1]);
                            }
                        }

                        return new RemoveDuplicatesCommand()
                        {
                            Logger = new ProjectLogger(),
                            PathLeft = pathLeft,
                            PathRight = pathRight,
                            Exporter = new ConsoleRemoveDuplicatesExporter(),
                            FileRemove = fileRemove
                        };
                    }

                default:
                    throw new Exception("Invalid command.");
            }
        }

        private static List<string> ReadBlackList(string filePath)
        {
            Console.WriteLine("Reading black list from file: {0}", filePath);

            return File.Exists(filePath)
                ? File.ReadAllLines(filePath)
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Where(x => !x.StartsWith("#"))
                    .ToList()
                : new List<string>();
        }
    }
}