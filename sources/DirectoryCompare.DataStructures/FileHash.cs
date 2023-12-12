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

namespace DustInTheWind.DirectoryCompare.DataStructures;

public readonly struct FileHash : IEquatable<FileHash>
{
    private readonly byte[] bytes;

    public FileHash(byte[] bytes)
    {
        this.bytes = bytes;
    }

    public override bool Equals(object obj)
    {
        return obj is FileHash other && Equals(other);
    }

    public bool Equals(FileHash other)
    {
        return AreEqual(bytes, other.bytes);
    }

    private static bool AreEqual(byte[] list1, byte[] list2)
    {  
        if (list1 == null || list2 == null)
            return false;

        if (list1.Length != list2.Length)
            return false;

        for (int i = 0; i < list1.Length; i++)
        {
            if (list1[i] != list2[i])
                return false;
        }

        return true;
    }

    public override int GetHashCode()
    {
        return bytes == null 
            ? 0
            : bytes.GetHashCode();
    }

    public static FileHash Parse(string value)
    {
        byte[] bytes = Convert.FromBase64String(value);
        return new FileHash(bytes);
    }

    public override string ToString()
    {
        return bytes == null
            ? null
            : Convert.ToBase64String(bytes);
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