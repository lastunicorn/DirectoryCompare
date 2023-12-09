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

public readonly partial struct DataSize
{
    /// <summary>
    /// Gets the value.
    /// </summary>
    public ulong Value { get; }

    /// <summary>
    /// Gets a value that specifies if the current instance represents zero bytes. 
    /// </summary>
    public bool IsZero => Value == 0;

    /// <summary>
    /// Gets the size value converted in Bytes.
    /// </summary>
    public ulong Bytes => Value;

    /// <summary>
    /// Gets the size value expressed in whole and fractional Kilobytes.
    /// </summary>
    public double Kilobytes => (double)Value / OneKilobyteValue;

    /// <summary>
    /// Gets the size value converted in whole Kilobytes. The value is rounded up.
    /// </summary>
    public ulong WholeKilobytes => (ulong)Math.Ceiling(Kilobytes);

    /// <summary>
    /// Gets the size value expressed in whole and fractional Megabytes.
    /// </summary>
    public double Megabytes => (double)Value / OneMegabyteValue;

    /// <summary>
    /// Gets the size value converted in whole Megabytes. The value is rounded up.
    /// </summary>
    public ulong WholeMegabytes => (ulong)Math.Ceiling(Megabytes);

    /// <summary>
    /// Gets the size value expressed in whole and fractional Gigabytes.
    /// </summary>
    public double Gigabytes => (double)Value / OneGigabyteValue;

    /// <summary>
    /// Gets the size value converted in whole Gigabytes. The value is rounded up.
    /// </summary>
    public ulong WholeGigabytes => (ulong)Math.Ceiling(Gigabytes);

    /// <summary>
    /// Gets the size value expressed in whole and fractional Terabytes.
    /// </summary>
    public double Terabytes => (double)Value / OneTerabyteValue;

    /// <summary>
    /// Gets the size value converted in whole Terabytes. The value is rounded up.
    /// </summary>
    public ulong WholeTerabytes => (ulong)Math.Ceiling(Terabytes);

    /// <summary>
    /// Gets the size value expressed in whole and fractional Petabytes.
    /// </summary>
    public double Petabytes => (double)Value / OnePetabyteValue;

    /// <summary>
    /// Gets the size value converted in whole Petabytes. The value is rounded up.
    /// </summary>
    public ulong WholePetabytes => (ulong)Math.Ceiling(Petabytes);
}