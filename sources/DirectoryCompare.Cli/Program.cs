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
using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Spinners;
using DustInTheWind.DirectoryCompare.Cli.Commands;
using DustInTheWind.DirectoryCompare.Cli.ResultExporters;

namespace DustInTheWind.DirectoryCompare.Cli
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                //args = new[] { "read-disk", @"c:\temp1", @"c:\temp\file1.json" };
                //args = new[] { "read-disk", @"c:\temp2", @"c:\temp\file2.json" };
                //args = new[] { "read-disk", @"c:\temp", @"c:\temp1\temp.json" };
                //args = new[] { "read-file", @"c:\temp1\temp.json" };
                //args = new[] { "verify-disk", @"c:\temp1", @"c:\temp\file1.json" };
                //args = new[] { "compare-disks", @"c:\temp1", @"c:\temp2" };
                //args = new[] { "compare-files", @"c:\temp\file1.json", @"c:\temp\file2.json" };

                //DisplayArguments(args);

                Arguments arguments = new Arguments(args);

                ICommand command = CreateProject(arguments);
                command.DisplayInfo();
                Spinner.Run(() => { command.Execute(); });

                CustomConsole.WriteLineSuccess("Done");
            }
            catch (Exception ex)
            {
                CustomConsole.WriteLineError(ex);
            }
        }

        private static void DisplayArguments(IEnumerable<string> args)
        {
            Console.WriteLine("Arguments:");

            foreach (string arg in args)
                Console.WriteLine(arg);

            Console.WriteLine();
        }

        private static ICommand CreateProject(Arguments arguments)
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