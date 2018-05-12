using System;
using System.IO;
using Newtonsoft.Json;

namespace DirectoryCompare
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

            switch (args[0])
            {
                case "read-disk":
                    Console.WriteLine("Reading path: " + args[1]);
                    ReadDisk(args[1], args[2]);
                    break;

                case "read-file":
                    Console.WriteLine("Reading file: " + args[1]);
                    ReadFile(args[1]);
                    break;

                case "verify-disk":
                    Console.WriteLine("Verify path: " + args[1]);
                    VerifyDisk(args[1], args[2]);
                    break;

                case "compare-disks":
                    Console.WriteLine("Compare paths:");
                    Console.WriteLine(args[1]);
                    Console.WriteLine(args[2]);
                    CompareDisks(args[1], args[2]);
                    break;

                case "compare-files":
                    CompareFiles(args[1], args[2]);
                    break;
            }

            Console.ReadKey(true);
        }

        private static void ReadFile(string filePath)
        {
            string json = File.ReadAllText(filePath);
            Container container = JsonConvert.DeserializeObject<Container>(json);

            ContainerView containerView = new ContainerView(container);
            containerView.Display();
        }

        private static void VerifyDisk(string diskPath, string filePath)
        {
            DiskReader diskReader1 = new DiskReader(diskPath);
            diskReader1.Read();

            string json2 = File.ReadAllText(filePath);
            Container container2 = JsonConvert.DeserializeObject<Container>(json2);

            Compare(diskReader1.Container, container2);
        }

        private static void CompareFiles(string path1, string path2)
        {
            string json1 = File.ReadAllText(path1);
            string json2 = File.ReadAllText(path2);
            Container container1 = JsonConvert.DeserializeObject<Container>(json1);
            Container container2 = JsonConvert.DeserializeObject<Container>(json2);

            Compare(container1, container2);
        }

        private static void CompareDisks(string path1, string path2)
        {
            DiskReader diskReader1 = new DiskReader(path1);
            diskReader1.Read();

            DiskReader diskReader2 = new DiskReader(path2);
            diskReader2.Read();

            Compare(diskReader1.Container, diskReader2.Container);
        }

        private static void ReadDisk(string sourcePath, string destinationFilePath)
        {
            DiskReader diskReader1 = new DiskReader(sourcePath);
            diskReader1.Read();

            string json = JsonConvert.SerializeObject(diskReader1.Container);
            File.WriteAllText(destinationFilePath, json);
        }

        private static void Compare(Container container1, Container container2)
        {
            ContainerComparer comparer = new ContainerComparer(container1, container2);
            comparer.Compare();

            Console.WriteLine();

            DisplayResults(comparer);
        }

        private static void DisplayResults(ContainerComparer comparer)
        {
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

    internal class ContainerView
    {
        private readonly Container container;

        public ContainerView(Container container)
        {
            this.container = container;
        }

        public void Display()
        {
            DisplayDirectory(container, 0);
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