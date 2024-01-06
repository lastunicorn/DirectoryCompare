// DirectoryCompare
// Copyright (C) 2017-2024 Dust in the Wind
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

public readonly partial struct DataSize
{
    public static implicit operator ulong(DataSize dataSize)
    {
        return dataSize.Value;
    }

    public static implicit operator DataSize(ulong bytes)
    {
        return new DataSize(bytes);
    }

    public static implicit operator DataSize(long bytes)
    {
        return new DataSize(bytes);
    }

    public static implicit operator DataSize(uint bytes)
    {
        return new DataSize(bytes);
    }

    public static implicit operator DataSize(int bytes)
    {
        return new DataSize(bytes);
    }

    public static implicit operator DataSize(ushort bytes)
    {
        return new DataSize(bytes);
    }

    public static implicit operator DataSize(short bytes)
    {
        return new DataSize(bytes);
    }
}