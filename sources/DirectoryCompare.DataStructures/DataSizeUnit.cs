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

/// <summary>
/// The measurement unit for data.
/// </summary>
public enum DataSizeUnit
{
    /// <summary>
    /// Unknown
    /// </summary>
    Unknown = 0,
    
    /// <summary>
    /// The byte is a group of 8 bits.
    /// </summary>
    Byte,
    
    // ---
    
    /// <summary>
    /// A mebibyte (KiB) is defined by international standard IEC 80000-13 to be equal to
    /// 2 ^ 10 bytes = 1.024 bytes.
    /// </summary>
    Kibibyte,
    
    /// <summary>
    /// A mebibyte (MiB) is defined by international standard IEC 80000-13 to be equal to
    /// 2 ^ 20 bytes = 1.048.576 bytes.
    /// </summary>
    Mebibyte,
    
    /// <summary>
    /// A gibibyte (GiB) is defined by international standard IEC 80000-13 to be equal to
    /// 2 ^ 30 bytes = 1.073.741.824 bytes.
    /// </summary>
    Gibibyte,
    
    /// <summary>
    /// A tebibyte (TiB) is defined by international standard IEC 80000-13 to be equal to
    /// 2 ^ 40 bytes = 1.099.511.627.776 bytes.
    /// </summary>
    Tebibyte,
    
    /// <summary>
    /// A pebibyte (PiB) is defined by international standard IEC 80000-13 to be equal to
    /// 2 ^ 50 bytes = 1.125.899.906.842.624 bytes.
    /// </summary>
    Pebibyte,
    
    // ---

    /// <summary>
    /// A kilobyte (kB) is defined based on the power of 10, similar to the units from the International System of Units (SI),
    /// and is equal to 10 ^ 3 bytes = 1.000 bytes.
    /// </summary>
    Kilobyte,
    
    /// <summary>
    /// A megabyte (MB) is defined based on the power of 10, similar to the units from the International System of Units (SI),
    /// and is equal to 10 ^ 6 bytes = 1.000.000 bytes.
    /// </summary>
    Megabyte,
    
    /// <summary>
    /// A gigabyte (GB) is defined based on the power of 10, similar to the units from the International System of Units (SI),
    /// and is equal to 10 ^ 9 bytes = 1.000.000.000 bytes.
    /// </summary>
    Gigabyte,
    
    /// <summary>
    /// A terabyte (TB) is defined based on the power of 10, similar to the units from the International System of Units (SI),
    /// and is equal to 10 ^ 12 bytes = 1.000.000.000.000 bytes.
    /// </summary>
    Terabyte,
    
    /// <summary>
    /// A petabyte (PB) is defined based on the power of 10, similar to the units from the International System of Units (SI),
    /// and is equal to 10 ^ 15 bytes = 1.000.000.000.000.000 bytes.
    /// </summary>
    Petabyte
}