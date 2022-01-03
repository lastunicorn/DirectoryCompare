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

namespace DustInTheWind.DirectoryCompare.Domain.Comparison
{
    public class FileDuplicates : IEnumerable<FileDuplicate>
    {
        public List<HFile> FilesLeft { get; set; }

        public List<HFile> FilesRight { get; set; }

        public bool CheckFilesExist { get; set; }

        public IEnumerator<FileDuplicate> GetEnumerator()
        {
            if (FilesLeft == null)
                yield break;

            if (FilesRight == null)
            {
                for (int i = 0; i < FilesLeft.Count; i++)
                {
                    HFile fileLeft = FilesLeft[i];

                    for (int j = i + 1; j < FilesLeft.Count; j++)
                    {
                        HFile fileRight = FilesLeft[j];

                        FileDuplicate fileDuplicate = new(fileLeft, fileRight, CheckFilesExist);

                        if (fileDuplicate.AreEqual)
                            yield return fileDuplicate;
                    }
                }
            }
            else
            {
                IEnumerable<FileDuplicate> duplicates = FilesLeft
                    .Select(fileLeft => FilesRight.Select(fileRight => new FileDuplicate(fileLeft, fileRight, CheckFilesExist)))
                    .SelectMany(fileDuplicates => fileDuplicates)
                    .Where(x => x.AreEqual);

                foreach (FileDuplicate fileDuplicate in duplicates)
                    yield return fileDuplicate;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}