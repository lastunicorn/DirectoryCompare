using System;
using System.Collections.Generic;
using System.Linq;

namespace DirectoryCompare
{
    internal class ContainerComparer
    {
        private readonly Container container1;
        private readonly Container container2;
        private readonly List<string> onlyInContainer1 = new List<string>();
        private readonly List<string> onlyInContainer2 = new List<string>();
        private readonly List<ItemComparison> differentNames = new List<ItemComparison>();
        private readonly List<ItemComparison> differentContent = new List<ItemComparison>();

        public IReadOnlyList<string> OnlyInContainer1 => onlyInContainer1;
        public IReadOnlyList<string> OnlyInContainer2 => onlyInContainer2;
        public IReadOnlyList<ItemComparison> DifferentNames => differentNames;
        public IReadOnlyList<ItemComparison> DifferentContent => differentContent;
        
        public ContainerComparer(Container container1, Container container2)
        {
            this.container1 = container1 ?? throw new ArgumentNullException(nameof(container1));
            this.container2 = container2 ?? throw new ArgumentNullException(nameof(container2));
        }

        public void Compare()
        {
            onlyInContainer1.Clear();
            onlyInContainer2.Clear();
            differentNames.Clear();
            differentContent.Clear();

            CompareDirectories(container1, container2, "/");
        }

        private void CompareDirectories(XDirectory xDirectory1, XDirectory xDirectory2, string rootPath)
        {
            CompareChildFiles(xDirectory1, xDirectory2, rootPath);
            CompareChildDirectories(xDirectory1, xDirectory2, rootPath);
        }

        private void CompareChildFiles(XDirectory xDirectory1, XDirectory xDirectory2, string rootPath)
        {
            List<XFile> files1 = xDirectory1.Files.ToList();
            List<XFile> files2 = xDirectory2.Files.ToList();

            foreach (XFile xFile1 in files1)
            {
                XFile xFile2 = files2.FirstOrDefault(x => x.Name == xFile1.Name || AreEqual(x.Hash, xFile1.Hash));

                if (xFile2 == null)
                {
                    onlyInContainer1.Add(rootPath + xFile1.Name);
                }
                else
                {
                    if (xFile1.Name != xFile2.Name)
                        differentNames.Add(new ItemComparison { RootPath = rootPath, Item1 = xFile1, Item2 = xFile2 });

                    if (!AreEqual(xFile1.Hash, xFile2.Hash))
                        differentContent.Add(new ItemComparison { RootPath = rootPath, Item1 = xFile1, Item2 = xFile2 });

                    files2.Remove(xFile2);
                }
            }

            foreach (XFile xFile2 in files2)
            {
                onlyInContainer2.Add(rootPath + xFile2.Name);
            }
        }

        private static bool AreEqual(IReadOnlyList<byte> list1, IReadOnlyList<byte> list2)
        {
            if (list1 == null || list2 == null)
                return false;

            if (list1.Count != list2.Count)
                return false;

            for (int i = 0; i < list1.Count; i++)
            {
                if (list1[i] != list2[i])
                    return false;
            }

            return true;
        }

        private void CompareChildDirectories(XDirectory xDirectory1, XDirectory xDirectory2, string rootPath)
        {
            List<XDirectory> subDirectories1 = xDirectory1.Directories.ToList();
            List<XDirectory> subDirectories2 = xDirectory2.Directories.ToList();

            foreach (XDirectory xSubDirectory1 in subDirectories1)
            {
                XDirectory xSubDirectory2 = subDirectories2.FirstOrDefault(x => x.Name == xSubDirectory1.Name);

                if (xSubDirectory2 == null)
                {
                    onlyInContainer1.Add(rootPath + xSubDirectory1.Name + "/");
                }
                else
                {
                    subDirectories2.Remove(xSubDirectory2);
                    CompareDirectories(xSubDirectory1, xSubDirectory2, rootPath + xSubDirectory1.Name + "/");
                }
            }

            foreach (XDirectory xSubDirectoy2 in subDirectories2)
            {
                onlyInContainer2.Add(rootPath + xSubDirectoy2.Name + "/");
            }
        }
    }
}