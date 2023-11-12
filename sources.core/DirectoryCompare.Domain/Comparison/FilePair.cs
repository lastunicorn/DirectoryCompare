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

public class FilePair
{
    private readonly HFile fileLeft;
    private readonly HFile fileRight;
    private readonly bool checkFilesExistence;
    private bool? areEqual;

    public bool AreEqual
    {
        get
        {
            areEqual ??= CalculateEquality();
            return areEqual.Value;
        }
    }

    public DataSize Size => fileLeft.Size;

    public string FullPathLeft => fileLeft.GetOriginalPath();

    public string FullPathRight => fileRight.GetOriginalPath();

    public bool FileLeftExists
    {
        get
        {
            string fullPath1 = fileLeft.GetOriginalPath();
            return File.Exists(fullPath1);
        }
    }

    public bool FileRightExists
    {
        get
        {
            string fullPath2 = fileRight.GetOriginalPath();
            return File.Exists(fullPath2);
        }
    }

    public FilePair(HFile fileLeft, HFile fileRight, bool checkFilesExistence)
    {
        this.fileLeft = fileLeft ?? throw new ArgumentNullException(nameof(fileLeft));
        this.fileRight = fileRight ?? throw new ArgumentNullException(nameof(fileRight));
        this.checkFilesExistence = checkFilesExistence;
    }

    private bool CalculateEquality()
    {
        bool filesAreEqual = fileLeft.Hash == fileRight.Hash && fileLeft.Size == fileRight.Size;

        return checkFilesExistence
            ? filesAreEqual && FileLeftExists && FileRightExists
            : filesAreEqual;
    }

    public void DeleteLeft()
    {
        File.Delete(FullPathLeft);
    }

    public void DeleteRight()
    {
        File.Delete(FullPathRight);
    }

    public void MoveLeft(string destinationDirectory)
    {
        MoveFile(fileLeft, destinationDirectory);
    }

    public void MoveRight(string destinationDirectory)
    {
        MoveFile(fileRight, destinationDirectory);
    }

    private static void MoveFile(HItem hFile, string destinationDirectory)
    {
        string sourceFilePath = hFile.GetOriginalPath();

        string relativePath = hFile.GetPath()
            .TrimStart(Path.DirectorySeparatorChar)
            .TrimStart(Path.AltDirectorySeparatorChar);
        string destinationFilePath = Path.Combine(destinationDirectory, relativePath);

        string destinationDirectoryPath = Path.GetDirectoryName(destinationFilePath);

        if (!Directory.Exists(destinationDirectoryPath))
            Directory.CreateDirectory(destinationDirectoryPath);

        File.Move(sourceFilePath, destinationFilePath);

        RemoveParentIfEmpty(sourceFilePath);
    }

    private static void RemoveParentIfEmpty(string path)
    {
        while (true)
        {
            string parentDirectoryPath = Path.GetDirectoryName(path);

            bool isDirectoryEmpty = !Directory.EnumerateFileSystemEntries(parentDirectoryPath).Any();

            if (!isDirectoryEmpty) return;

            Directory.Delete(parentDirectoryPath);
            path = parentDirectoryPath;
        }
    }
}