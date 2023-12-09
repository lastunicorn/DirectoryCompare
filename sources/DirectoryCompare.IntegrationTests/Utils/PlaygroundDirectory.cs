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

namespace DustInTheWind.DirectoryCompare.IntegrationTests.Utils;

internal class PlaygroundDirectory : IDisposable
{
    private const string Path = "CrawlerTestsPlayground";

    public PlaygroundDirectory()
    {
        if (Directory.Exists(Path))
        {
            bool isDirectoryEmpty = IsDirectoryEmpty();
            if (!isDirectoryEmpty)
                throw new Exception($"The directory intended to be used for the crawling tests is not empty: '{Path}'");
        }
        else
        {
            Directory.CreateDirectory(Path);
        }
    }

    private bool IsDirectoryEmpty()
    {
        return !Directory.EnumerateFileSystemEntries(Path).Any();
    }

    public string CombineWith(params string[] paths)
    {
        string[] allPaths = new[] { Path }
            .Concat(paths)
            .ToArray();

        return System.IO.Path.Combine(allPaths);
    }

    public void CreateChildDirectory(params string[] directoryNames)
    {
        string[] paths = new[] { Path }
            .Concat(directoryNames)
            .ToArray();

        string fullPath = System.IO.Path.Combine(paths);
        Directory.CreateDirectory(fullPath);
    }

    public void Dispose()
    {
        Directory.Delete(Path, true);
    }

    public static implicit operator string(PlaygroundDirectory _)
    {
        return Path;
    }

    public void CreateChildFile(params string[] paths)
    {
        string[] allPaths = new[] { Path }
            .Concat(paths)
            .ToArray();

        string fullPath = System.IO.Path.Combine(allPaths);
        string directoryPath = System.IO.Path.GetDirectoryName(fullPath);
        Directory.CreateDirectory(directoryPath);

        string content = GenerateFileContent();
        File.WriteAllText(fullPath, content);
    }

    private string GenerateFileContent()
    {
        return "file content";
    }
}