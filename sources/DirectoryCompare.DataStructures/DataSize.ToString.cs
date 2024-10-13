// Directory Compare
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
    /// Returns a string representation of the size in bytes.
    /// </summary>
    public override string ToString()
    {
        return ToStringBinary();
    }

    public string ToStringBinary()
    {
        double n = value;

        if (n >= OnePebibyteValue)
        {
            n /= OnePebibyteValue;

            return n < 10
                ? $"{n:N2} PiB"
                : $"{n:N0} PiB";
        }

        if (n >= OneTebibyteValue)
        {
            n /= OneTebibyteValue;

            return n < 10
                ? $"{n:N2} TiB"
                : $"{n:N0} TiB";
        }

        if (n >= OneGibibyteValue)
        {
            n /= OneGibibyteValue;

            return n < 10
                ? $"{n:N2} GiB"
                : $"{n:N0} GiB";
        }

        if (n >= OneMebibyteValue)
        {
            n /= OneMebibyteValue;

            return n < 10
                ? $"{n:N2} MiB"
                : $"{n:N0} MiB";
        }

        if (n >= OneKibibyteValue)
        {
            n /= OneKibibyteValue;

            return n < 10
                ? $"{n:N2} KiB"
                : $"{n:N0} KiB";
        }

        return $"{n:N0} B";
    }

    public string ToStringDecimal()
    {
        double n = value;

        if (n >= OnePetabyteValue)
        {
            n /= OnePetabyteValue;

            return n < 10
                ? $"{n:N2} PB"
                : $"{n:N0} PB";
        }

        if (n >= OneTerabyteValue)
        {
            n /= OneTerabyteValue;

            return n < 10
                ? $"{n:N2} TB"
                : $"{n:N0} TB";
        }

        if (n >= OneGigabyteValue)
        {
            n /= OneGigabyteValue;

            return n < 10
                ? $"{n:N2} GB"
                : $"{n:N0} GB";
        }

        if (n >= OneMegabyteValue)
        {
            n /= OneMegabyteValue;

            return n < 10
                ? $"{n:N2} MB"
                : $"{n:N0} MB";
        }

        if (n >= OneKilobyteValue)
        {
            n /= OneKilobyteValue;

            return n < 10
                ? $"{n:N2} KB"
                : $"{n:N0} KB";
        }

        return $"{n:N0} B";
    }

    /// <summary>
    /// Returns a string representation of the instance, using the specified format.
    /// </summary>
    /// 
    /// <param name="format">
    /// Possible values:
    /// "S" OR "simple" = string representation of the value in bytes;
    /// "D" OR "detailed" = string representation of the value in both, the most convenient unit and bytes.
    /// </param>
    ///
    /// <returns>A string representation of the current instance.</returns>
    public string ToString(string format)
    {
        return ToString(format, null);
    }

    /// <summary>
    /// Returns a string representation of the instance, using the specified format.
    /// </summary>
    /// 
    /// <param name="format">
    /// Possible values:
    /// "S" OR "simple" = string representation of the value in bytes;
    /// "D" OR "detailed" = string representation of the value in both, the most convenient unit and bytes.
    /// </param>
    /// 
    /// <param name="formatProvider"></param>
    /// 
    /// <returns>A string representation of the current instance.</returns>
    public string ToString(string format, IFormatProvider formatProvider)
    {
        if (format is "S2" or "simple-binary")
            return ToStringBinary() ?? string.Empty;

        if (format is "S10" or "simple-decimal")
            return ToStringDecimal() ?? string.Empty;

        if (format is "D2" or "detailed-binary")
        {
            string simple = ToStringBinary();
            string asBytes = ToString(DataSizeUnit.Byte);

            return $"{simple} ({asBytes})";
        }

        if (format is "D10" or "detailed-decimal")
        {
            string simple = ToStringDecimal();
            string asBytes = ToString(DataSizeUnit.Byte);

            return $"{simple} ({asBytes})";
        }

        return ToStringBinary() ?? string.Empty;
    }

    /// <summary>
    /// Returns a string representation of the size value converted in the specified measurement unit.
    /// </summary>
    public string ToString(DataSizeUnit unit)
    {
        switch (unit)
        {
            case DataSizeUnit.Unknown:
            case DataSizeUnit.Byte:
                return $"{value:N0} B";

            // ---

            case DataSizeUnit.Kibibyte:
                return $"{Kibibytes:N0} KiB";

            case DataSizeUnit.Mebibyte:
                return $"{Mebibytes:N0} MiB";

            case DataSizeUnit.Gibibyte:
                return $"{Gibibytes:N0} GiB";

            case DataSizeUnit.Tebibyte:
                return $"{Tebibytes:N0} TiB";

            case DataSizeUnit.Pebibyte:
                return $"{Pebibytes:N0} PiB";

            // ---

            case DataSizeUnit.Kilobyte:
                return $"{Kilobytes:N0} KB";

            case DataSizeUnit.Megabyte:
                return $"{Megabytes:N0} MB";

            case DataSizeUnit.Gigabyte:
                return $"{Gigabytes:N0} GB";

            case DataSizeUnit.Terabyte:
                return $"{Terabytes:N0} TB";

            case DataSizeUnit.Petabyte:
                return $"{Petabytes:N0} PB";

            // ---

            default:
                throw new ArgumentOutOfRangeException(nameof(unit), unit, null);
        }
    }
}