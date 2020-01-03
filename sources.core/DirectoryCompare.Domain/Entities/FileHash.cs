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
using System.Collections.Generic;

namespace DustInTheWind.DirectoryCompare.Domain.Entities
{
    public struct FileHash : IEquatable<FileHash>
    {
        private readonly byte[] bytes;

        public FileHash(byte[] bytes)
        {
            this.bytes = bytes;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public bool Equals(FileHash other)
        {
            return AreEqual(bytes, other.bytes);
        }

        public override int GetHashCode()
        {
            return (bytes != null ? bytes.GetHashCode() : 0);
        }

        public static bool AreEqual(IReadOnlyList<byte> list1, IReadOnlyList<byte> list2)
        {
            if (list1 == null || list2 == null)
                return false;

            if (list1.Count != list2.Count)
                return false;

            for (int i = 0; i < list1.Count; i++)
            {
                if (list1[i] != list2[i])
                    return false;
            }

            return true;
        }

        public override string ToString()
        {
            return Convert.ToBase64String(bytes);
        }

        public static bool operator ==(FileHash fileHash1, FileHash fileHash2)
        {
            return fileHash1.Equals(fileHash2);
        }

        public static bool operator !=(FileHash fileHash1, FileHash fileHash2)
        {
            return !fileHash1.Equals(fileHash2);
        }

        public static implicit operator byte[](FileHash fileHash)
        {
            return fileHash.bytes;
        }

        public static implicit operator FileHash(byte[] bytes)
        {
            return new FileHash(bytes);
        }
    }
}