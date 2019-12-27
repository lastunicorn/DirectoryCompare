using System;
using System.Buffers.Text;
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