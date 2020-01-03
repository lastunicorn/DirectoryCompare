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

using DustInTheWind.DirectoryCompare.Domain.Entities;
using System.Collections;
using System.Collections.Generic;

namespace DustInTheWind.DirectoryCompare.Domain.Comparison
{
    public class FileDuplicates : IEnumerable<FileDuplicate>
    {
        public List<HFile> FilesLeft { get; set; }

        public List<HFile> FilesRight { get; set; }

        public bool CheckFilesExist { get; set; }

        public IEnumerator<FileDuplicate> GetEnumerator()
        {
            if (FilesRight == null)
            {
                for (int i = 0; i < FilesLeft.Count; i++)
                {
                    HFile fileLeft = FilesLeft[i];

                    for (int j = i + 1; j < FilesLeft.Count; j++)
                    {
                        HFile fileRight = FilesLeft[j];

                        FileDuplicate fileDuplicate = new FileDuplicate(fileLeft, fileRight, CheckFilesExist);

                        if (fileDuplicate.AreEqual)
                            yield return fileDuplicate;
                    }
                }
            }
            else
            {
                foreach (HFile fileLeft in FilesLeft)
                {
                    foreach (HFile fileRight in FilesRight)
                    {
                        FileDuplicate fileDuplicate = new FileDuplicate(fileLeft, fileRight, CheckFilesExist);

                        if (fileDuplicate.AreEqual)
                            yield return fileDuplicate;
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}