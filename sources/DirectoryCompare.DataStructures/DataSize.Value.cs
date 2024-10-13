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
    /// Gets the value.
    /// </summary>
    private readonly ulong value;

    /// <summary>
    /// Gets a value that specifies if the current instance represents zero bytes. 
    /// </summary>
    public bool IsZero => value == 0;

    /// <summary>
    /// Gets the size value converted in bytes.
    /// </summary>
    public ulong Bytes => value;

    // ---
    
    /// <summary>
    /// Gets the size value expressed in whole and fractional kibibytes.
    /// </summary>
    public double Kibibytes => (double)value / OneKibibyteValue;

    /// <summary>
    /// Gets the size value converted in whole kibibytes. The value is rounded up.
    /// </summary>
    public ulong WholeKibibytes => (ulong)Math.Ceiling(Kibibytes);

    /// <summary>
    /// Gets the size value expressed in whole and fractional mebibytes.
    /// </summary>
    public double Mebibytes => (double)value / OneMebibyteValue;

    /// <summary>
    /// Gets the size value converted in whole mebibytes. The value is rounded up.
    /// </summary>
    public ulong WholeMebibytes => (ulong)Math.Ceiling(Mebibytes);

    /// <summary>
    /// Gets the size value expressed in whole and fractional gibibytes.
    /// </summary>
    public double Gibibytes => (double)value / OneGibibyteValue;

    /// <summary>
    /// Gets the size value converted in whole gibibytes. The value is rounded up.
    /// </summary>
    public ulong WholeGibibytes => (ulong)Math.Ceiling(Gibibytes);

    /// <summary>
    /// Gets the size value expressed in whole and fractional tebibytes.
    /// </summary>
    public double Tebibytes => (double)value / OneTebibyteValue;

    /// <summary>
    /// Gets the size value converted in whole tebibytes. The value is rounded up.
    /// </summary>
    public ulong WholeTebibytes => (ulong)Math.Ceiling(Tebibytes);

    /// <summary>
    /// Gets the size value expressed in whole and fractional pebibytes.
    /// </summary>
    public double Pebibytes => (double)value / OnePebibyteValue;

    /// <summary>
    /// Gets the size value converted in whole pebibytes. The value is rounded up.
    /// </summary>
    public ulong WholePebibytes => (ulong)Math.Ceiling(Pebibytes);
    
    // ---
    
    /// <summary>
    /// Gets the size value expressed in whole and fractional kilobytes.
    /// </summary>
    public double Kilobytes => (double)value / OneKilobyteValue;
    
    /// <summary>
    /// Gets the size value converted in whole kilobytes. The value is rounded up.
    /// </summary>
    public ulong WholeKilobytes => (ulong)Math.Ceiling(Kilobytes);
    
    /// <summary>
    /// Gets the size value expressed in whole and fractional megabytes.
    /// </summary>
    public double Megabytes => (double)value / OneMegabyteValue;
    
    /// <summary>
    /// Gets the size value converted in whole megabytes. The value is rounded up.
    /// </summary>
    public ulong WholeMegabytes => (ulong)Math.Ceiling(Megabytes);
    
    /// <summary>
    /// Gets the size value expressed in whole and fractional gigabytes.
    /// </summary>
    public double Gigabytes => (double)value / OneGigabyteValue;
    
    /// <summary>
    /// Gets the size value converted in whole gigabytes. The value is rounded up.
    /// </summary>
    public ulong WholeGigabytes => (ulong)Math.Ceiling(Gigabytes);
    
    /// <summary>
    /// Gets the size value expressed in whole and fractional terabytes.
    /// </summary>
    public double Terabytes => (double)value / OneTerabyteValue;
    
    /// <summary>
    /// Gets the size value converted in whole terabytes. The value is rounded up.
    /// </summary>
    public ulong WholeTerabytes => (ulong)Math.Ceiling(Terabytes);
    
    /// <summary>
    /// Gets the size value expressed in whole and fractional petabytes.
    /// </summary>
    public double Petabytes => (double)value / OnePetabyteValue;
    
    /// <summary>
    /// Gets the size value converted in whole petabytes. The value is rounded up.
    /// </summary>
    public ulong WholePetabytes => (ulong)Math.Ceiling(Petabytes);
}