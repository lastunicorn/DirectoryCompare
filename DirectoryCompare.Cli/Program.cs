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

namespace DustInTheWind.DirectoryCompare
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //args = new[] { "read-disk", @"c:\temp1", @"c:\temp\file1.json" };
            //args = new[] { "read-disk", @"c:\temp2", @"c:\temp\file2.json" };
            //args = new[] { "read-disk", @"c:\temp", @"c:\temp1\temp.json" };
            //args = new[] { "read-file", @"c:\temp1\temp.json" };
            //args = new[] { "verify-disk", @"c:\temp1", @"c:\temp\file1.json" };
            //args = new[] { "compare-disks", @"c:\temp1", @"c:\temp2" };
            //args = new[] { "compare-files", @"c:\temp\file1.json", @"c:\temp\file2.json" };

            Console.WriteLine("Arguments:");

            foreach (string arg in args)
                Console.WriteLine(arg);

            Console.WriteLine();

            Project project = CreateProject(args);
            project.Run();

            Console.ReadKey(true);
        }

        private static Project CreateProject(string[] args)
        {
            switch (args[0])
            {
                case "read-disk":
                    Console.WriteLine("Reading path: " + args[1]);
                    return new Project
                    {
                        Command = new ReadDiskCommand
                        {
                            SourcePath = args[1],
                            DestinationFilePath = args[2]
                        }
                    };

                case "read-file":
                    Console.WriteLine("Reading file: " + args[1]);
                    return new Project
                    {
                        Command = new ReadFileCommand
                        {
                            FilePath = args[1]
                        }
                    };

                case "verify-disk":
                    Console.WriteLine("Verify path: " + args[1]);
                    return new Project
                    {
                        Command = new VerifyDiskCommand
                        {
                            DiskPath = args[1],
                            FilePath = args[2]
                        }
                    };

                case "compare-disks":
                    Console.WriteLine("Compare paths:");
                    Console.WriteLine(args[1]);
                    Console.WriteLine(args[2]);
                    return new Project
                    {
                        Command = new CompareDisksCommand
                        {
                            Path1 = args[1],
                            Path2 = args[2]
                        }
                    };

                case "compare-files":
                    return new Project
                    {
                        Command = new CompareFilesCommand
                        {
                            Path1 = args[1],
                            Path2 = args[2]
                        }
                    };

                default:
                    throw new Exception("Invalid command.");
            }
        }

        public static void DisplayResults(ContainerComparer comparer)
        {
            Console.WriteLine();

            Console.WriteLine("Files only in container 1:");
            foreach (string path in comparer.OnlyInContainer1)
                Console.WriteLine(path);

            Console.WriteLine();

            Console.WriteLine("Files only in container 2:");
            foreach (string path in comparer.OnlyInContainer2)
                Console.WriteLine(path);

            Console.WriteLine();

            Console.WriteLine("Different names:");
            foreach (ItemComparison itemComparison in comparer.DifferentNames)
            {
                Console.WriteLine("1 - " + itemComparison.FullName1);
                Console.WriteLine("2 - " + itemComparison.FullName2);
            }

            Console.WriteLine();

            Console.WriteLine("Different content:");
            foreach (ItemComparison itemComparison in comparer.DifferentContent)
            {
                Console.WriteLine("1 - " + itemComparison.FullName1);
                Console.WriteLine("2 - " + itemComparison.FullName2);
            }
        }
    }
}