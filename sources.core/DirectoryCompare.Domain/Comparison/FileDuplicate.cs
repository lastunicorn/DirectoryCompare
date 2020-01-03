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
using System;
using System.IO;

namespace DustInTheWind.DirectoryCompare.Domain.Comparison
{
    public class FileDuplicate
    {
        private readonly HFile file1;
        private readonly HFile file2;
        private readonly bool checkFilesExist;
        private bool? areEqual;

        public bool AreEqual
        {
            get
            {
                if (!areEqual.HasValue)
                    CalculateEquality();

                return areEqual.Value;
            }
        }

        public long Size => file1.Size;
        public string FullPath1 => file1.GetOriginalPath();
        public string FullPath2 => file2.GetOriginalPath();

        public bool File1Exists
        {
            get
            {
                string fullPath1 = file1.GetOriginalPath();
                return File.Exists(fullPath1);
            }
        }

        public bool File2Exists
        {
            get
            {
                string fullPath2 = file2.GetOriginalPath();
                return File.Exists(fullPath2);
            }
        }

        public FileDuplicate(HFile file1, HFile file2, bool checkFilesExist)
        {
            this.file1 = file1 ?? throw new ArgumentNullException(nameof(file1));
            this.file2 = file2 ?? throw new ArgumentNullException(nameof(file2));
            this.checkFilesExist = checkFilesExist;
        }

        private void CalculateEquality()
        {
            if (file1.Hash == file2.Hash && file1.Size == file2.Size)
            {
                areEqual = false;

                if (checkFilesExist)
                {
                    if (File1Exists && File2Exists)
                        areEqual = true;
                }
                else
                {
                    areEqual = true;
                }
            }
            else
            {
                areEqual = false;
            }
        }
    }
}