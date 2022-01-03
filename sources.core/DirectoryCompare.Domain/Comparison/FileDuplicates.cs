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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Domain.Utils;

namespace DustInTheWind.DirectoryCompare.Domain.Comparison
{
    public class FileDuplicates : IEnumerable<FileDuplicate>
    {
        public List<HFile> FilesLeft { get; set; }

        public List<HFile> FilesRight { get; set; }

        public bool CheckFilesExistance { get; set; }

        public int DuplicateCount { get; private set; }

        public DataSize TotalSize { get; private set; }

        public IEnumerator<FileDuplicate> GetEnumerator()
        {
            DuplicateCount = 0;
            TotalSize = 0;

            if (FilesLeft == null)
                yield break;

            IEnumerable<FileDuplicate> duplicates = FilesRight == null
                ? GeneratePairs(FilesLeft)
                : GeneratePairs(FilesLeft, FilesRight);

            duplicates = duplicates
                .Where(x => x.AreEqual);

            foreach (FileDuplicate fileDuplicate in duplicates)
            {
                DuplicateCount++;
                TotalSize += fileDuplicate.Size;
                yield return fileDuplicate;
            }
        }

        private IEnumerable<FileDuplicate> GeneratePairs(IReadOnlyList<HFile> files)
        {
            for (int i = 0; i < files.Count; i++)
            {
                HFile fileLeft = files[i];

                for (int j = i + 1; j < files.Count; j++)
                {
                    HFile fileRight = files[j];

                    yield return new FileDuplicate(fileLeft, fileRight, CheckFilesExistance);
                }
            }
        }

        private IEnumerable<FileDuplicate> GeneratePairs(IEnumerable<HFile> filesLeft, IEnumerable<HFile> filesRight)
        {
            return filesLeft
                .SelectMany(fileLeft => filesRight.Select(fileRight => new FileDuplicate(fileLeft, fileRight, CheckFilesExistance)));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}