// DirectoryCompare
// Copyright (C) 2017-2020 Dust in the Wind
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

            CompareChildFiles(Snapshot1, Snapshot2, "/");
            CompareChildDirectories(Snapshot1, Snapshot2, "/");
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
        List<HFile> onlyInDirectory2 = files2.ToList();

        foreach (HFile file1 in files1)
        {
            List<HFile> file2Matches = files2
                .Where(x => x.Name == file1.Name || x.Hash == file1.Hash)
                .ToList();

            if (file2Matches.Count == 0)
            {
                onlyInSnapshot1.Add(rootPath + file1.Name);
            }
            else
            {
                foreach (HFile file2 in file2Matches)
                {
                    ItemComparison itemComparison = new()
                    {
                        RootPath = rootPath,
                        Item1 = file1,
                        Item2 = file2
                    };

                    bool sameName = file1.Name == file2.Name;
                    bool sameContent = file1.Hash == file2.Hash;

                    if (!sameName && sameContent)
                        differentNames.Add(itemComparison);

                    if (sameName && !sameContent)
                        differentContent.Add(itemComparison);

                    onlyInDirectory2.Remove(file2);
                }
            }
        }

        foreach (HFile file2 in onlyInDirectory2)
        {
            onlyInSnapshot2.Add(rootPath + file2.Name);
        }
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