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
    private const ulong OneKibibyteValue = 1024;
    private const ulong OneMebibyteValue = OneKibibyteValue * 1024;
    private const ulong OneGibibyteValue = OneMebibyteValue * 1024;
    private const ulong OneTebibyteValue = OneGibibyteValue * 1024;
    private const ulong OnePebibyteValue = OneTebibyteValue * 1024;
    
    private const ulong OneKilobyteValue = 1000;
    private const ulong OneMegabyteValue = OneKilobyteValue * 1000;
    private const ulong OneGigabyteValue = OneMegabyteValue * 1000;
    private const ulong OneTerabyteValue = OneGigabyteValue * 1000;
    private const ulong OnePetabyteValue = OneTerabyteValue * 1000;

    /// <summary>
    /// Gets a <see cref="DataSize"/> instance that represents zero bytes.
    /// </summary>
    public static DataSize Zero { get; } = new(0);

    /// <summary>
    /// Gets a <see cref="DataSize"/> instance that represents one byte.
    /// </summary>
    public static DataSize OneByte { get; } = new(1);

    // ---
    
    /// <summary>
    /// Gets a <see cref="DataSize"/> instance that represents one kibibyte (2 ^ 10 bytes).
    /// </summary>
    public static DataSize OneKibibyte { get; } = new(OneKibibyteValue);

    /// <summary>
    /// Gets a <see cref="DataSize"/> instance that represents one mebibyte (2 ^ 20 bytes).
    /// </summary>
    public static DataSize OneMebibyte { get; } = new(OneMebibyteValue);

    /// <summary>
    /// Gets a <see cref="DataSize"/> instance that represents one gibibyte (2 ^ 30 bytes).
    /// </summary>
    public static DataSize OneGibibyte { get; } = new(OneGibibyteValue);

    /// <summary>
    /// Gets a <see cref="DataSize"/> instance that represents one tebibyte (2 ^ 40 bytes).
    /// </summary>
    public static DataSize OneTebibyte { get; } = new(OneTebibyteValue);
    
    /// <summary>
    /// Gets a <see cref="DataSize"/> instance that represents one pebibyte (2 ^ 50 bytes).
    /// </summary>
    public static DataSize OnePebibyte { get; } = new(OnePebibyteValue);
    
    // ---

    /// <summary>
    /// Gets a <see cref="DataSize"/> instance that represents one kilobyte (10 ^ 3 bytes).
    /// </summary>
    public static DataSize OneKilobyte { get; } = new(OneKilobyteValue);

    /// <summary>
    /// Gets a <see cref="DataSize"/> instance that represents one megabyte (10 ^ 6 bytes).
    /// </summary>
    public static DataSize OneMegabyte { get; } = new(OneMegabyteValue);

    /// <summary>
    /// Gets a <see cref="DataSize"/> instance that represents one gigabyte (10 ^ 9 bytes).
    /// </summary>
    public static DataSize OneGigabyte { get; } = new(OneGigabyteValue);

    /// <summary>
    /// Gets a <see cref="DataSize"/> instance that represents one terabyte (10 ^ 12 bytes).
    /// </summary>
    public static DataSize OneTerabyte { get; } = new(OneTerabyteValue);
    
    /// <summary>
    /// Gets a <see cref="DataSize"/> instance that represents one petabyte (10 ^ 15 bytes).
    /// </summary>
    public static DataSize OnePetabyte { get; } = new(OnePetabyteValue);
}