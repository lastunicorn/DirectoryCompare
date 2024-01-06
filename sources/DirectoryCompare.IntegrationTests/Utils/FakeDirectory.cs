// DirectoryCompare
// Copyright (C) 2017-2024 Dust in the Wind
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

namespace DustInTheWind.DirectoryCompare.IntegrationTests.Utils;

internal class FakeDirectory
{
    private readonly string rootPath;

    public string Name { get; }

    public FakeDirectory Parent { get; set; }

    public List<FakeFile> Files { get; } = new();

    public List<FakeDirectory> Directories { get; } = new();

    public FakeDirectory(string name)
    {
        rootPath = Path.GetDirectoryName(name);
        Name = Path.GetRandomFileName();
    }

    public string GetFullPath()
    {
        if (Parent == null)
        {
            return rootPath != null
                ? Path.Combine(rootPath, Name)
                : Name;
        }

        string parentFullPath = Parent.GetFullPath();
        return Path.Combine(parentFullPath, Name);
    }

    public string CombineWith(params string[] paths)
    {
        string[] allPaths = new[] { GetFullPath() }
            .Concat(paths)
            .ToArray();

        return Path.Combine(allPaths);
    }

    public void CreateChildDirectories(params string[] directoryNames)
    {
        IEnumerable<FakeDirectory> childDirectories = directoryNames
            .Select(x => new FakeDirectory(x));

        Directories.AddRange(childDirectories);
    }

    public FakeDirectory CreateChildDirectory(params string[] names)
    {
        if (names.Length == 0)
            return this;

        FakeDirectory fakeDirectory = new(names[0])
        {
            Parent = this
        };
        Directories.Add(fakeDirectory);

        if (names.Length == 1)
            return fakeDirectory;

        string[] remainingNames = names.Skip(1).ToArray();
        return fakeDirectory.CreateChildDirectory(remainingNames);
    }

    public void CreateChildFiles(params string[] fileNames)
    {
        IEnumerable<FakeFile> childFiles = fileNames
            .Select(x => new FakeFile(x));

        Files.AddRange(childFiles);
    }

    public FakeFile CreateChildFile(params string[] names)
    {
        if (names.Length == 0)
            return null;

        FakeDirectory fakeDirectory;

        if (names.Length > 1)
        {
            string[] directoryNames = names.SkipLast(1).ToArray();
            fakeDirectory = CreateChildDirectory(directoryNames);
        }
        else
        {
            fakeDirectory = this;
        }

        string fileName = names.Last();

        FakeFile fakeFile = new(fileName)
        {
            Parent = fakeDirectory
        };
        Files.Add(fakeFile);

        return fakeFile;
    }

    public static implicit operator string(FakeDirectory fakeDirectory)
    {
        return fakeDirectory.GetFullPath();
    }
}