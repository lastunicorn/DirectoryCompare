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

using DustInTheWind.DirectoryCompare.Utils;
using System;
using System.IO;

namespace DustInTheWind.DirectoryCompare.Cli
{
    internal class Duplicate
    {
        private readonly Tuple<string, HFile> tuple1;
        private readonly Tuple<string, HFile> tuple2;
        private readonly bool checkFilesExist;
        private readonly HContainer hContainer1;
        private readonly HContainer hContainer2;
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

        public Duplicate(Tuple<string, HFile> tuple1, Tuple<string, HFile> tuple2, bool checkFilesExist, HContainer hContainer1, HContainer hContainer2)
        {
            this.tuple1 = tuple1 ?? throw new ArgumentNullException(nameof(tuple1));
            this.tuple2 = tuple2 ?? throw new ArgumentNullException(nameof(tuple2));
            this.checkFilesExist = checkFilesExist;
            this.hContainer1 = hContainer1 ?? throw new ArgumentNullException(nameof(hContainer1));
            this.hContainer2 = hContainer2 ?? throw new ArgumentNullException(nameof(hContainer2));
        }

        private void CalculateEquality()
        {
            bool areEqual = ByteArrayCompare.AreEqual(tuple1.Item2.Hash, tuple2.Item2.Hash);

            if (areEqual)
            {
                this.areEqual = false;

                string path1 = tuple1.Item1;
                string path2 = tuple2.Item1;

                FullPath1 = Path.Combine(hContainer1.OriginalPath, path1.Substring(1));
                FullPath2 = Path.Combine(hContainer2.OriginalPath, path2.Substring(1));

                File1Exists = File.Exists(FullPath1);
                File2Exists = File.Exists(FullPath2);

                if (checkFilesExist)
                {
                    if (File1Exists && File2Exists)
                    {
                        this.areEqual = true;
                        Size = new FileInfo(FullPath2).Length;
                    }
                }
                else
                {
                    this.areEqual = true;

                    if (File1Exists)
                    {
                        Size = new FileInfo(FullPath1).Length;
                    }
                    else if (File2Exists)
                    {
                        Size = new FileInfo(FullPath2).Length;
                    }
                    else
                    {
                        Size = 0;
                    }
                }
            }
            else
            {
                this.areEqual = false;
            }
        }
    }
}
