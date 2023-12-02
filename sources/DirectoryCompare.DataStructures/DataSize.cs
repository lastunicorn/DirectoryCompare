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

using System.Text.RegularExpressions;

namespace DustInTheWind.DirectoryCompare.DataStructures;

/// <summary>
/// Represents the size of some data.
/// It contains a value and a measurement unit.
/// </summary>
public struct DataSize : IEquatable<DataSize>, IFormattable
{
    private const ulong OneKiloByte = 1024;
    private const ulong OneMegaByte = OneKiloByte * 1024;
    private const ulong OneGigaByte = OneMegaByte * 1024;
    private const ulong OneTeraByte = OneGigaByte * 1024;

    private static readonly Regex Regex = new(@"^\s*(\d+\.?\d*)\s*(b|kb|mb|gb|tb)*\s*$", RegexOptions.IgnoreCase);

    /// <summary>
    /// Gets the value.
    /// </summary>
    public ulong Value { get; }

    /// <summary>
    /// Gets an empty <see cref="DataSize"/>.
    /// </summary>
    public static DataSize Zero { get; } = new(0);

    public static DataSize OneByte { get; } = new((ulong)1);

    public static DataSize OneKilobyte { get; } = new((ulong)1024);

    public static DataSize OneMegabyte { get; } = new((ulong)1024 * 1024);

    public static DataSize OneGigabyte { get; } = new((ulong)1024 * 1024 * 1024);

    public static DataSize OneTerabyte { get; } = new((ulong)1024 * 1024 * 1024 * 1024);

    public bool IsZero => Value == 0;

    /// <summary>
    /// Gets the value converted in Bytes.
    /// </summary>
    public ulong Bytes => Value;

    /// <summary>
    /// Gets the size expressed in whole and fractional Kilobytes.
    /// </summary>
    public double Kilobytes => (double)Value / 1024;

    /// <summary>
    /// Gets the value converted in whole Kilobytes. The value is rounded up.
    /// </summary>
    public ulong WholeKilobytes => (ulong)Math.Ceiling(Kilobytes);

    /// <summary>
    /// Gets the size expressed in whole and fractional Megabytes.
    /// </summary>
    public double Megabytes => (double)Value / 1024 / 1024;

    /// <summary>
    /// Gets the value converted in whole Megabytes. The value is rounded up.
    /// </summary>
    public ulong WholeMegabytes => (ulong)Math.Ceiling(Megabytes);

    /// <summary>
    /// Gets the size expressed in whole and fractional Gigabytes.
    /// </summary>
    public double Gigabytes => (double)Value / 1024 / 1024 / 1024;

    /// <summary>
    /// Gets the value converted in whole Gigabytes. The value is rounded up.
    /// </summary>
    public ulong WholeGigabytes => (ulong)Math.Ceiling(Gigabytes);

    /// <summary>
    /// Gets the size expressed in whole and fractional Terabytes.
    /// </summary>
    public double Terabytes => (double)Value / 1024 / 1024 / 1024 / 1024;

    /// <summary>
    /// Gets the value converted in whole Terabytes. The value is rounded up.
    /// </summary>
    public ulong WholeTerabytes => (ulong)Math.Ceiling(Terabytes);

    /// <summary>
    /// Initializes a new instance of the <see cref="DataSize"/> struct.
    /// </summary>
    public DataSize(ulong value, DataSizeUnit unit = DataSizeUnit.Byte)
    {
        Value = ToBytes(value, unit);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataSize"/> struct.
    /// </summary>
    public DataSize(double value, DataSizeUnit unit = DataSizeUnit.Byte)
    {
        Value = ToBytes(value, unit);
    }

    /// <summary>
    /// Returns another <see cref="DataSize"/> object with the value converted in Bytes.
    /// </summary>
    private static ulong ToBytes(ulong value, DataSizeUnit unit)
    {
        switch (unit)
        {
            case DataSizeUnit.Unknown:
            case DataSizeUnit.Byte:
                return value;

            case DataSizeUnit.Kilobyte:
                return value * 1024;

            case DataSizeUnit.Megabyte:
                return value * 1024 * 1024;

            case DataSizeUnit.Gigabyte:
                return value * 1024 * 1024 * 1024;

            case DataSizeUnit.Terabyte:
                return value * 1024 * 1024 * 1024 * 1024;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// Returns another <see cref="DataSize"/> object with the value converted in Bytes.
    /// </summary>
    private static ulong ToBytes(double value, DataSizeUnit unit)
    {
        if (value < 0)
            return 0;

        switch (unit)
        {
            case DataSizeUnit.Unknown:
            case DataSizeUnit.Byte:
                return (ulong)Math.Round(value);

            case DataSizeUnit.Kilobyte:
                return (ulong)Math.Round(value) * 1024;

            case DataSizeUnit.Megabyte:
                return (ulong)Math.Round(value) * 1024 * 1024;

            case DataSizeUnit.Gigabyte:
                return (ulong)Math.Round(value) * 1024 * 1024 * 1024;

            case DataSizeUnit.Terabyte:
                return (ulong)Math.Round(value) * 1024 * 1024 * 1024 * 1024;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    #region From

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

    #endregion

    #region Parse

    /// <summary>
    /// Parses the specified text and creates a <see cref="DataSize"/> object with the obtained values.
    /// </summary>
    public static DataSize Parse(string text)
    {
        Match match = Regex.Match(text);

        if (!match.Success)
            throw new ArgumentException("The text is not a string representation of a data size.", nameof(text));

        double value = double.Parse(match.Groups[1].Value);
        DataSizeUnit unit = ParseUnit(match.Groups[2].Value);

        return new DataSize(value, unit);
    }

    /// <summary>
    /// Parses the specified text and returns a <see cref="DataSizeUnit"/> value.
    /// </summary>
    public static DataSizeUnit ParseUnit(string text)
    {
        if (text == null) throw new ArgumentNullException(nameof(text));

        bool isByte = text.Trim().Equals("b", StringComparison.InvariantCultureIgnoreCase);
        if (isByte)
            return DataSizeUnit.Byte;

        bool isKilobyte = text.Trim().Equals("kb", StringComparison.InvariantCultureIgnoreCase);
        if (isKilobyte)
            return DataSizeUnit.Kilobyte;

        bool isMegabyte = text.Trim().Equals("mb", StringComparison.InvariantCultureIgnoreCase);
        if (isMegabyte)
            return DataSizeUnit.Megabyte;

        bool isGigabyte = text.Trim().Equals("gb", StringComparison.InvariantCultureIgnoreCase);
        if (isGigabyte)
            return DataSizeUnit.Gigabyte;

        bool isTerabyte = text.Trim().Equals("tb", StringComparison.InvariantCultureIgnoreCase);
        if (isTerabyte)
            return DataSizeUnit.Terabyte;

        return DataSizeUnit.Unknown;
    }

    #endregion

    #region Operators

    public static DataSize operator *(DataSize dataSize1, float dataSize2)
    {
        return new DataSize(dataSize1.Value * dataSize2);
    }

    public static DataSize operator *(float dataSize1, DataSize dataSize2)
    {
        return new DataSize(dataSize2.Value * dataSize1);
    }

    public static DataSize operator -(DataSize dataSize1, float dataSize2)
    {
        return new DataSize(dataSize1.Value - dataSize2);
    }

    public static DataSize operator -(float dataSize1, DataSize dataSize2)
    {
        return new DataSize(dataSize2.Value - dataSize1);
    }

    public static DataSize operator -(DataSize dataSize1, DataSize dataSize2)
    {
        return new DataSize(dataSize1.Value - dataSize2.Value);
    }

    public static DataSize operator +(DataSize dataSize1, float dataSize2)
    {
        return new DataSize(dataSize1.Value + dataSize2);
    }

    public static DataSize operator +(float dataSize1, DataSize dataSize2)
    {
        return new DataSize(dataSize2.Value + dataSize1);
    }

    public static DataSize operator +(DataSize dataSize1, DataSize dataSize2)
    {
        return new DataSize(dataSize1.Value + dataSize2.Value);
    }

    public static bool operator <(DataSize dataSize1, DataSize dataSize2)
    {
        return dataSize1.Value < dataSize2.Value;
    }

    public static bool operator >(DataSize dataSize1, DataSize dataSize2)
    {
        return dataSize1.Value > dataSize2.Value;
    }

    public static bool operator <=(DataSize dataSize1, DataSize dataSize2)
    {
        return dataSize1.Value <= dataSize2.Value;
    }

    public static bool operator >=(DataSize dataSize1, DataSize dataSize2)
    {
        return dataSize1.Value >= dataSize2.Value;
    }

    public static bool operator <(DataSize dataSize1, ulong dataSize2)
    {
        return dataSize1.Value < dataSize2;
    }

    public static bool operator >(DataSize dataSize1, ulong dataSize2)
    {
        return dataSize1.Value > dataSize2;
    }

    public static bool operator <=(DataSize dataSize1, ulong dataSize2)
    {
        return dataSize1.Value <= dataSize2;
    }

    public static bool operator >=(DataSize dataSize1, ulong dataSize2)
    {
        return dataSize1.Value >= dataSize2;
    }

    public static bool operator <(DataSize dataSize1, int dataSize2)
    {
        if (dataSize2 < 0)
            return false;

        return dataSize1.Value < (ulong)dataSize2;
    }

    public static bool operator >(DataSize dataSize1, int dataSize2)
    {
        if (dataSize2 < 0)
            return true;

        return dataSize1.Value > (ulong)dataSize2;
    }

    public static bool operator <=(DataSize dataSize1, int dataSize2)
    {
        if (dataSize2 < 0)
            return false;

        return dataSize1.Value <= (ulong)dataSize2;
    }

    public static bool operator >=(DataSize dataSize1, int dataSize2)
    {
        if (dataSize2 < 0)
            return true;

        return dataSize1.Value >= (ulong)dataSize2;
    }

    #endregion

    #region Implicit Operators

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

    #endregion

    #region Equals

    public bool Equals(DataSize other)
    {
        return Value.Equals(other.Value);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        return obj is DataSize other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    #endregion

    #region ToString

    /// <summary>
    /// Returns a string representation of the size in bytes.
    /// </summary>
    public override string ToString()
    {
        double tempSize = Value;

        if (tempSize >= OneTeraByte)
        {
            tempSize /= OneTeraByte;

            return tempSize < 10
                ? $"{tempSize:N2} TB"
                : $"{tempSize:N0} TB";
        }

        if (tempSize >= OneGigaByte)
        {
            tempSize /= OneGigaByte;

            return tempSize < 10
                ? $"{tempSize:N2} GB"
                : $"{tempSize:N0} GB";
        }

        if (tempSize >= OneMegaByte)
        {
            tempSize /= OneMegaByte;

            return tempSize < 10
                ? $"{tempSize:N2} MB"
                : $"{tempSize:N0} MB";
        }

        if (tempSize >= OneKiloByte)
        {
            tempSize /= OneKiloByte;

            return tempSize < 10
                ? $"{tempSize:N2} KB"
                : $"{tempSize:N0} KB";
        }

        return $"{tempSize:N0} B";
    }

    public string ToString(string format)
    {
        return ToString(format, null);
    }

    public string ToString(string format, IFormatProvider formatProvider)
    {
        if (format is "S" or "simple")
        {
            return ToString() ?? string.Empty;
        }

        if (format is "D" or "detailed")
        {
            string simple = ToString();
            string asBytes = ToString(DataSizeUnit.Byte);

            return $"{simple} ({asBytes})";
        }

        return ToString() ?? string.Empty;
    }

    /// <summary>
    /// Returns a string representation of the size converted in the specified measurement unit.
    /// </summary>
    public string ToString(DataSizeUnit unit)
    {
        switch (unit)
        {
            case DataSizeUnit.Unknown:
            case DataSizeUnit.Byte:
                return $"{Value:N0} B";

            case DataSizeUnit.Kilobyte:
                return $"{Kilobytes:N0} KB";

            case DataSizeUnit.Megabyte:
                return $"{Megabytes:N0} MB";

            case DataSizeUnit.Gigabyte:
                return $"{Gigabytes:N0} GB";

            case DataSizeUnit.Terabyte:
                return $"{Terabytes:N0} TB";

            default:
                throw new ArgumentOutOfRangeException(nameof(unit), unit, null);
        }
    }

    #endregion
}