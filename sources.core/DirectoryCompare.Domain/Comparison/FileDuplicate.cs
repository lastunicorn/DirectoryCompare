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

using System;
using System.IO;
using DustInTheWind.DirectoryCompare.Domain.Entities;

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

        public long Size { get; private set; }
        public string FullPath1 { get; private set; }
        public string FullPath2 { get; private set; }
        public bool File1Exists { get; private set; }
        public bool File2Exists { get; private set; }

        public FileDuplicate(HFile file1, HFile file2, bool checkFilesExist)
        {
            this.file1 = file1 ?? throw new ArgumentNullException(nameof(file1));
            this.file2 = file2 ?? throw new ArgumentNullException(nameof(file2));
            this.checkFilesExist = checkFilesExist;
        }

        private void CalculateEquality()
        {
            if (file1.Hash == file2.Hash)
            {
                areEqual = false;

                FullPath1 = file1.GetOriginalPath();
                FullPath2 = file2.GetOriginalPath();

                File1Exists = File.Exists(FullPath1);
                File2Exists = File.Exists(FullPath2);

                if (checkFilesExist)
                {
                    if (File1Exists && File2Exists)
                    {
                        areEqual = true;
                        Size = new FileInfo(FullPath2).Length;
                    }
                }
                else
                {
                    areEqual = true;

                    if (File1Exists)
                        Size = new FileInfo(FullPath1).Length;
                    else if (File2Exists)
                        Size = new FileInfo(FullPath2).Length;
                    else
                        Size = 0;
                }
            }
            else
            {
                areEqual = false;
            }
        }
    }
}