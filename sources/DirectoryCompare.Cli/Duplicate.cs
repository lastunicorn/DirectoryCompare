// DirectoryCompare
// Copyright (C) 2017 Dust in the Wind
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
        private readonly Tuple<string, XFile> tuple1;
        private readonly Tuple<string, XFile> tuple2;
        private readonly bool checkFilesExist;
        private readonly XContainer xContainer;
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

        public Duplicate(Tuple<string, XFile> tuple1, Tuple<string, XFile> tuple2, bool checkFilesExist, XContainer xContainer)
        {
            this.tuple1 = tuple1 ?? throw new ArgumentNullException(nameof(tuple1));
            this.tuple2 = tuple2 ?? throw new ArgumentNullException(nameof(tuple2));
            this.checkFilesExist = checkFilesExist;
            this.xContainer = xContainer ?? throw new ArgumentNullException(nameof(xContainer));
        }

        private void CalculateEquality()
        {
            bool areEqual = ByteArrayCompare.AreEqual(tuple1.Item2.Hash, tuple2.Item2.Hash);

            if (areEqual)
            {
                this.areEqual = false;

                string path1 = tuple1.Item1;
                string path2 = tuple2.Item1;

                FullPath1 = Path.Combine(xContainer.OriginalPath, path1.Substring(1));
                FullPath2 = Path.Combine(xContainer.OriginalPath, path2.Substring(1));

                if (checkFilesExist)
                {
                    if (File.Exists(FullPath1))
                    {
                        if (File.Exists(FullPath2))
                        {
                            this.areEqual = true;
                            Size = new FileInfo(FullPath2).Length;
                        }
                    }
                }
                else
                {
                    this.areEqual = true;

                    if (File.Exists(FullPath1))
                    {
                        Size = new FileInfo(FullPath1).Length;
                    }
                    else if (File.Exists(FullPath2))
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
