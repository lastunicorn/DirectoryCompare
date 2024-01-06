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
/// Represents the size of some data.
/// It contains a value and a measurement unit.
/// </summary>
public readonly partial struct DataSize
{
    private const ulong OneKilobyteValue = 1024;
    private const ulong OneMegabyteValue = OneKilobyteValue * 1024;
    private const ulong OneGigabyteValue = OneMegabyteValue * 1024;
    private const ulong OneTerabyteValue = OneGigabyteValue * 1024;
    private const ulong OnePetabyteValue = OneTerabyteValue * 1024;

    /// <summary>
    /// Gets a <see cref="DataSize"/> instance that represents zero bytes.
    /// </summary>
    public static DataSize Zero { get; } = new(0);

    /// <summary>
    /// Gets a <see cref="DataSize"/> instance that represents one byte.
    /// </summary>
    public static DataSize OneByte { get; } = new(1);

    /// <summary>
    /// Gets a <see cref="DataSize"/> instance that represents one kilobyte.
    /// </summary>
    public static DataSize OneKilobyte { get; } = new(OneKilobyteValue);

    /// <summary>
    /// Gets a <see cref="DataSize"/> instance that represents one megabyte.
    /// </summary>
    public static DataSize OneMegabyte { get; } = new(OneMegabyteValue);

    /// <summary>
    /// Gets a <see cref="DataSize"/> instance that represents one gigabyte.
    /// </summary>
    public static DataSize OneGigabyte { get; } = new(OneGigabyteValue);

    /// <summary>
    /// Gets a <see cref="DataSize"/> instance that represents one terabyte.
    /// </summary>
    public static DataSize OneTerabyte { get; } = new(OneTerabyteValue);
    
    /// <summary>
    /// Gets a <see cref="DataSize"/> instance that represents one petabyte.
    /// </summary>
    public static DataSize OnePetabyte { get; } = new(OnePetabyteValue);
}