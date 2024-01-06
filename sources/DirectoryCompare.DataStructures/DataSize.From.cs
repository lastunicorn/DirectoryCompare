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
    /// <summary>
    /// Creates a new <see cref="DataSize"/> with the specified value in Bytes.
    /// </summary>
    public static DataSize FromBytes(ulong value)
    {
        return new DataSize(value);
    }

    /// <summary>
    /// Creates a new <see cref="DataSize"/> with the specified value in Kilobytes.
    /// </summary>
    public static DataSize FromKilobytes(double value)
    {
        return new DataSize(value, DataSizeUnit.Kilobyte);
    }

    /// <summary>
    /// Creates a new <see cref="DataSize"/> with the specified value in Megabytes.
    /// </summary>
    public static DataSize FromMegabytes(double value)
    {
        return new DataSize(value, DataSizeUnit.Megabyte);
    }

    /// <summary>
    /// Creates a new <see cref="DataSize"/> with the specified value in Gigabytes.
    /// </summary>
    public static DataSize FromGigabytes(double value)
    {
        return new DataSize(value, DataSizeUnit.Gigabyte);
    }

    /// <summary>
    /// Creates a new <see cref="DataSize"/> with the specified value in Terabytes.
    /// </summary>
    public static DataSize FromTerabytes(double value)
    {
        return new DataSize(value, DataSizeUnit.Terabyte);
    }

    /// <summary>
    /// Creates a new <see cref="DataSize"/> with the specified value in Petabytes.
    /// </summary>
    public static DataSize FromPetabytes(double value)
    {
        return new DataSize(value, DataSizeUnit.Petabyte);
    }
}