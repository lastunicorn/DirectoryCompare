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

using DustInTheWind.DirectoryCompare.Domain.Entities;

namespace DustInTheWind.DirectoryCompare.Domain.Comparison;

public class FileDuplicatesOld
{
    public List<HFile> FilesLeft { get; set; }

    public List<HFile> FilesRight { get; set; }

    public IEnumerable<FilePair> Enumerate()
    {
        if (FilesLeft == null)
            return Enumerable.Empty<FilePair>();

        return GeneratePairs();
    }

    private IEnumerable<FilePair> GeneratePairs()
    {
        bool isOnlyOneList = FilesRight == null || ReferenceEquals(FilesRight, FilesLeft);

        return isOnlyOneList
            ? GeneratePairs(FilesLeft)
            : GeneratePairs(FilesLeft, FilesRight);
    }

    private static IEnumerable<FilePair> GeneratePairs(IReadOnlyList<HFile> files)
    {
        for (int i = 0; i < files.Count; i++)
        {
            HFile fileLeft = files[i];

            for (int j = i + 1; j < files.Count; j++)
            {
                HFile fileRight = files[j];
                FilePair filePair = new(fileLeft, fileRight);

                if (filePair.AreEqual)
                    yield return filePair;
            }
        }
    }

    private static IEnumerable<FilePair> GeneratePairs(IReadOnlyList<HFile> filesLeft, IReadOnlyList<HFile> filesRight)
    {
        for (int i = 0; i < filesLeft.Count; i++)
        {
            HFile fileLeft = filesLeft[i];

            for (int j = 0; j < filesRight.Count; j++)
            {
                HFile fileRight = filesRight[j];
                FilePair filePair = new(fileLeft, fileRight);

                if (filePair.AreEqual)
                    yield return filePair;
            }
        }
    }
}