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
using System.Linq;

namespace DustInTheWind.DirectoryCompare
{
    public class ContainerComparer
    {
        public Container Container1 { get; }
        public Container Container2 { get; }

        public DateTime StartTimeUtc { get; private set; }
        public DateTime EndTimeUtc { get; private set; }
        public TimeSpan TotalTime => EndTimeUtc - StartTimeUtc;

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
            Container1 = container1 ?? throw new ArgumentNullException(nameof(container1));
            Container2 = container2 ?? throw new ArgumentNullException(nameof(container2));
        }

        public void Compare()
        {
            StartTimeUtc = DateTime.UtcNow;

            try
            {
                onlyInContainer1.Clear();
                onlyInContainer2.Clear();
                differentNames.Clear();
                differentContent.Clear();

                CompareDirectories(Container1, Container2, "/");
            }
            finally
            {
                EndTimeUtc = DateTime.UtcNow;
            }
        }

        private void CompareDirectories(XDirectory xDirectory1, XDirectory xDirectory2, string rootPath)
        {
            CompareChildFiles(xDirectory1, xDirectory2, rootPath);
            CompareChildDirectories(xDirectory1, xDirectory2, rootPath);
        }

        private void CompareChildFiles(XDirectory xDirectory1, XDirectory xDirectory2, string rootPath)
        {
            List<XFile> files1 = xDirectory1.Files;
            List<XFile> files2 = xDirectory2.Files;
            List<XFile> onlyInDirectory2 = xDirectory2.Files.ToList();

            foreach (XFile xFile1 in files1)
            {
                List<XFile> xFile2Matches = files2
                    .Where(x => x.Name == xFile1.Name || AreEqual(x.Hash, xFile1.Hash))
                    .ToList();

                if (xFile2Matches.Count == 0)
                {
                    onlyInContainer1.Add(rootPath + xFile1.Name);
                }
                else
                {
                    foreach (XFile xFile2 in xFile2Matches)
                    {
                        if (xFile1.Name != xFile2.Name && AreEqual(xFile1.Hash, xFile2.Hash))
                            differentNames.Add(new ItemComparison { RootPath = rootPath, Item1 = xFile1, Item2 = xFile2 });

                        if (xFile1.Name == xFile2.Name && !AreEqual(xFile1.Hash, xFile2.Hash))
                            differentContent.Add(new ItemComparison { RootPath = rootPath, Item1 = xFile1, Item2 = xFile2 });

                        onlyInDirectory2.Remove(xFile2);
                    }
                }
            }

            foreach (XFile xFile2 in onlyInDirectory2)
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
            List<XDirectory> subDirectories1 = xDirectory1.Directories?.ToList() ?? new List<XDirectory>();
            List<XDirectory> subDirectories2 = xDirectory2.Directories?.ToList() ?? new List<XDirectory>();

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