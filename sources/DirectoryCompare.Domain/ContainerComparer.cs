// DirectoryCompare
// Copyright (C) 2017-2019 Dust in the Wind
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
using DustInTheWind.DirectoryCompare.Common.Utils;
using DustInTheWind.DirectoryCompare.Entities;

namespace DustInTheWind.DirectoryCompare
{
    public class ContainerComparer
    {
        public HContainer Container1 { get; }
        public HContainer Container2 { get; }

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

        public ContainerComparer(HContainer hContainer1, HContainer hContainer2)
        {
            Container1 = hContainer1 ?? throw new ArgumentNullException(nameof(hContainer1));
            Container2 = hContainer2 ?? throw new ArgumentNullException(nameof(hContainer2));
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

        private void CompareDirectories(HDirectory hDirectory1, HDirectory hDirectory2, string rootPath)
        {
            CompareChildFiles(hDirectory1, hDirectory2, rootPath);
            CompareChildDirectories(hDirectory1, hDirectory2, rootPath);
        }

        private void CompareChildFiles(HDirectory hDirectory1, HDirectory hDirectory2, string rootPath)
        {
            List<HFile> files1 = hDirectory1.Files ?? new List<HFile>();
            List<HFile> files2 = hDirectory2.Files ?? new List<HFile>();
            List<HFile> onlyInDirectory2 = hDirectory2.Files?.ToList() ?? new List<HFile>();

            foreach (HFile xFile1 in files1)
            {
                List<HFile> xFile2Matches = files2
                    .Where(x => x.Name == xFile1.Name || ByteArrayCompare.AreEqual(x.Hash, xFile1.Hash))
                    .ToList();

                if (xFile2Matches.Count == 0)
                {
                    onlyInContainer1.Add(rootPath + xFile1.Name);
                }
                else
                {
                    foreach (HFile xFile2 in xFile2Matches)
                    {
                        if (xFile1.Name != xFile2.Name && ByteArrayCompare.AreEqual(xFile1.Hash, xFile2.Hash))
                            differentNames.Add(new ItemComparison { RootPath = rootPath, Item1 = xFile1, Item2 = xFile2 });

                        if (xFile1.Name == xFile2.Name && !ByteArrayCompare.AreEqual(xFile1.Hash, xFile2.Hash))
                            differentContent.Add(new ItemComparison { RootPath = rootPath, Item1 = xFile1, Item2 = xFile2 });

                        onlyInDirectory2.Remove(xFile2);
                    }
                }
            }

            foreach (HFile xFile2 in onlyInDirectory2)
            {
                onlyInContainer2.Add(rootPath + xFile2.Name);
            }
        }

        private void CompareChildDirectories(HDirectory hDirectory1, HDirectory hDirectory2, string rootPath)
        {
            List<HDirectory> subDirectories1 = hDirectory1.Directories?.ToList() ?? new List<HDirectory>();
            List<HDirectory> subDirectories2 = hDirectory2.Directories?.ToList() ?? new List<HDirectory>();

            foreach (HDirectory xSubDirectory1 in subDirectories1)
            {
                HDirectory hSubDirectory2 = subDirectories2.FirstOrDefault(x => x.Name == xSubDirectory1.Name);

                if (hSubDirectory2 == null)
                {
                    onlyInContainer1.Add(rootPath + xSubDirectory1.Name + "/");
                }
                else
                {
                    subDirectories2.Remove(hSubDirectory2);
                    CompareDirectories(xSubDirectory1, hSubDirectory2, rootPath + xSubDirectory1.Name + "/");
                }
            }

            foreach (HDirectory xSubDirectory2 in subDirectories2)
            {
                onlyInContainer2.Add(rootPath + xSubDirectory2.Name + "/");
            }
        }
    }
}