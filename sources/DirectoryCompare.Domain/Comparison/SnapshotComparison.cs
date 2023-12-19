// DirectoryCompare
// Copyright (C) 2017-2023 Dust in the Wind
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

using DustInTheWind.DirectoryCompare.DataStructures;
using DustInTheWind.DirectoryCompare.Domain.Entities;

namespace DustInTheWind.DirectoryCompare.Domain.Comparison;

public class SnapshotComparison
{
    private readonly List<string> onlyInSnapshot1 = new();
    private readonly List<string> onlyInSnapshot2 = new();
    private readonly List<ItemComparison> differentNames = new();
    private readonly List<ItemComparison> differentContent = new();

    public Snapshot Snapshot1 { get; }

    public Snapshot Snapshot2 { get; }

    public SnapshotPath Path1 { get; set; }

    public SnapshotPath Path2 { get; set; }

    public DateTime StartTimeUtc { get; private set; }

    public DateTime EndTimeUtc { get; private set; }

    public TimeSpan TotalTime => EndTimeUtc - StartTimeUtc;

    public IReadOnlyList<string> OnlyInSnapshot1 => onlyInSnapshot1;

    public IReadOnlyList<string> OnlyInSnapshot2 => onlyInSnapshot2;

    public IReadOnlyList<ItemComparison> DifferentNames => differentNames;

    public IReadOnlyList<ItemComparison> DifferentContent => differentContent;

    public SnapshotComparison(Snapshot snapshot1, Snapshot snapshot2)
    {
        Snapshot1 = snapshot1 ?? throw new ArgumentNullException(nameof(snapshot1));
        Snapshot2 = snapshot2 ?? throw new ArgumentNullException(nameof(snapshot2));
    }

    public void Compare()
    {
        StartTimeUtc = DateTime.UtcNow;

        try
        {
            onlyInSnapshot1.Clear();
            onlyInSnapshot2.Clear();
            differentNames.Clear();
            differentContent.Clear();

            HDirectory hDirectory1 = Snapshot1.GetDirectory(Path1);
            HDirectory hDirectory2 = Snapshot2.GetDirectory(Path2);

            CompareChildFiles(hDirectory1, hDirectory2, "/");
            CompareChildDirectories(hDirectory1, hDirectory2, "/");
        }
        finally
        {
            EndTimeUtc = DateTime.UtcNow;
        }
    }

    private void CompareChildFiles(HDirectory directory1, HDirectory directory2, string rootPath)
    {
        HItemCollection<HFile> files1 = directory1.Files;
        HItemCollection<HFile> files2 = directory2.Files;

        List<HFile> remainingInDirectory2 = files2.ToList();

        foreach (HFile file1 in files1)
        {
            List<FileComparison> matches = files2
                .Select(x => new FileComparison(file1, x))
                .Where(x => x.IsSomeMatch)
                .ToList();

            if (matches.Count == 0)
            {
                onlyInSnapshot1.Add(rootPath + file1.Name);
            }
            else
            {
                foreach (FileComparison match in matches)
                {
                    bool isPartialMatch = match is
                    {
                        IsPerfectMatch: false,
                        IsSomeMatch: true
                    };

                    if (isPartialMatch)
                    {
                        ItemComparison itemComparison = new()
                        {
                            RootPath = rootPath,
                            Item1 = match.File1,
                            Item2 = match.File2
                        };

                        if (!match.SameName)
                            differentNames.Add(itemComparison);

                        if (!match.SameContent)
                            differentContent.Add(itemComparison);
                    }

                    remainingInDirectory2.Remove(match.File2);
                }
            }
        }

        foreach (HFile file2 in remainingInDirectory2)
            onlyInSnapshot2.Add(rootPath + file2.Name);
    }

    private void CompareChildDirectories(HDirectory directory1, HDirectory directory2, string rootPath)
    {
        List<HDirectory> subDirectories1 = directory1.Directories.ToList();
        List<HDirectory> subDirectories2 = directory2.Directories.ToList();

        foreach (HDirectory subDirectory1 in subDirectories1)
        {
            HDirectory subDirectory2 = subDirectories2.FirstOrDefault(x => x.Name == subDirectory1.Name);

            if (subDirectory2 == null)
            {
                onlyInSnapshot1.Add(rootPath + subDirectory1.Name + "/");
            }
            else
            {
                subDirectories2.Remove(subDirectory2);

                string rootPath1 = rootPath + subDirectory1.Name + "/";

                CompareChildFiles(subDirectory1, subDirectory2, rootPath1);
                CompareChildDirectories(subDirectory1, subDirectory2, rootPath1);
            }
        }

        foreach (HDirectory subDirectory2 in subDirectories2)
        {
            onlyInSnapshot2.Add(rootPath + subDirectory2.Name + "/");
        }
    }
}