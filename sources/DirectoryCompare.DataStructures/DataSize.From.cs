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
    /// Creates a new <see cref="DataSize"/> with the specified value.
    /// </summary>
    public static DataSize From(ulong value, DataSizeUnit unit)
    {
        return new DataSize(value, unit);
    }
    
    /// <summary>
    /// Creates a new <see cref="DataSize"/> with the specified value.
    /// </summary>
    public static DataSize From(double value, DataSizeUnit unit)
    {
        return new DataSize(value, unit);
    }
    
    // ---

    /// <summary>
    /// Creates a new <see cref="DataSize"/> with the specified value in bytes.
    /// </summary>
    public static DataSize FromBytes(ulong value)
    {
        return new DataSize(value);
    }

    // ---

    /// <summary>
    /// Creates a new <see cref="DataSize"/> with the specified value in kibibytes.
    /// </summary>
    public static DataSize FromKibibytes(double value)
    {
        return new DataSize(value, DataSizeUnit.Kibibyte);
    }

    /// <summary>
    /// Creates a new <see cref="DataSize"/> with the specified value in mebibytes.
    /// </summary>
    public static DataSize FromMebibytes(double value)
    {
        return new DataSize(value, DataSizeUnit.Mebibyte);
    }

    /// <summary>
    /// Creates a new <see cref="DataSize"/> with the specified value in gibibytes.
    /// </summary>
    public static DataSize FromGibibytes(double value)
    {
        return new DataSize(value, DataSizeUnit.Gibibyte);
    }

    /// <summary>
    /// Creates a new <see cref="DataSize"/> with the specified value in tebibytes.
    /// </summary>
    public static DataSize FromTebibytes(double value)
    {
        return new DataSize(value, DataSizeUnit.Tebibyte);
    }

    /// <summary>
    /// Creates a new <see cref="DataSize"/> with the specified value in pebibytes.
    /// </summary>
    public static DataSize FromPebibytes(double value)
    {
        return new DataSize(value, DataSizeUnit.Pebibyte);
    }
    
    // ---

    /// <summary>
    /// Creates a new <see cref="DataSize"/> with the specified value in kilobytes.
    /// </summary>
    public static DataSize FromKilobytes(double value)
    {
        return new DataSize(value, DataSizeUnit.Kilobyte);
    }

    /// <summary>
    /// Creates a new <see cref="DataSize"/> with the specified value in megabytes.
    /// </summary>
    public static DataSize FromMegabytes(double value)
    {
        return new DataSize(value, DataSizeUnit.Megabyte);
    }

    /// <summary>
    /// Creates a new <see cref="DataSize"/> with the specified value in gigabytes.
    /// </summary>
    public static DataSize FromGigabytes(double value)
    {
        return new DataSize(value, DataSizeUnit.Gigabyte);
    }

    /// <summary>
    /// Creates a new <see cref="DataSize"/> with the specified value in terabytes.
    /// </summary>
    public static DataSize FromTerabytes(double value)
    {
        return new DataSize(value, DataSizeUnit.Terabyte);
    }

    /// <summary>
    /// Creates a new <see cref="DataSize"/> with the specified value in petabytes.
    /// </summary>
    public static DataSize FromPetabytes(double value)
    {
        return new DataSize(value, DataSizeUnit.Petabyte);
    }
}