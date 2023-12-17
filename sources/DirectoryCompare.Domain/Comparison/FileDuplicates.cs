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

public class FileDuplicates
{
    Dictionary<FileHash, List<HFile>> filesByHash = new();

    public List<HFile> FilesLeft { get; set; }

    public List<HFile> FilesRight { get; set; }

    public IEnumerable<FilePair> Enumerate()
    {
        if (FilesLeft == null)
            yield break;

        filesByHash.Clear();

        bool existsRight = FilesRight != null && !ReferenceEquals(FilesRight, FilesLeft);

        if (existsRight)
            AddAllFilesWithoutCheck(FilesRight);

        IEnumerable<FilePair> duplicates = AddFilesAndCheckForDuplicates(FilesLeft);

        foreach (FilePair filePair in duplicates)
            yield return filePair;
    }

    private IEnumerable<FilePair> AddFilesAndCheckForDuplicates(List<HFile> filesToAdd)
    {
        foreach (HFile hFile in filesToAdd)
        {
            List<HFile> bucket = GetBucketFor(hFile.Hash);

            if (bucket.Count > 0)
            {
                foreach (HFile existingFile in bucket)
                    yield return new FilePair(hFile, existingFile);
            }
            
            bucket.Add(hFile);
        }
    }

    private void AddAllFilesWithoutCheck(List<HFile> filesToAdd)
    {
        foreach (HFile hFile in filesToAdd)
        {
            List<HFile> bucket = GetBucketFor(hFile.Hash);
            bucket.Add(hFile);
        }
    }

    private List<HFile> GetBucketFor(FileHash fileHash)
    {
        bool exists = filesByHash.TryGetValue(fileHash, out List<HFile> bucket);

        if (!exists)
        {
            bucket = new List<HFile>();
            filesByHash.Add(fileHash, bucket);
        }

        return bucket;
    }
}