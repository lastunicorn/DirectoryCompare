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
    /// Returns a string representation of the size in bytes.
    /// </summary>
    public override string ToString()
    {
        double tempSize = Value;

        if (tempSize >= OnePetabyteValue)
        {
            tempSize /= OnePetabyteValue;

            return tempSize < 10
                ? $"{tempSize:N2} PiB"
                : $"{tempSize:N0} PiB";
        }

        if (tempSize >= OneTerabyteValue)
        {
            tempSize /= OneTerabyteValue;

            return tempSize < 10
                ? $"{tempSize:N2} TiB"
                : $"{tempSize:N0} TiB";
        }

        if (tempSize >= OneGigabyteValue)
        {
            tempSize /= OneGigabyteValue;

            return tempSize < 10
                ? $"{tempSize:N2} GiB"
                : $"{tempSize:N0} GiB";
        }

        if (tempSize >= OneMegabyteValue)
        {
            tempSize /= OneMegabyteValue;

            return tempSize < 10
                ? $"{tempSize:N2} MiB"
                : $"{tempSize:N0} MiB";
        }

        if (tempSize >= OneKilobyteValue)
        {
            tempSize /= OneKilobyteValue;

            return tempSize < 10
                ? $"{tempSize:N2} KiB"
                : $"{tempSize:N0} KiB";
        }

        return $"{tempSize:N0} B";
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
    /// Returns a string representation of the size value converted in the specified measurement unit.
    /// </summary>
    public string ToString(DataSizeUnit unit)
    {
        switch (unit)
        {
            case DataSizeUnit.Unknown:
            case DataSizeUnit.Byte:
                return $"{Value:N0} B";

            case DataSizeUnit.Kilobyte:
                return $"{Kilobytes:N0} KiB";

            case DataSizeUnit.Megabyte:
                return $"{Megabytes:N0} MiB";

            case DataSizeUnit.Gigabyte:
                return $"{Gigabytes:N0} GiB";

            case DataSizeUnit.Terabyte:
                return $"{Terabytes:N0} TiB";

            case DataSizeUnit.Petabyte:
                return $"{Terabytes:N0} PiB";

            default:
                throw new ArgumentOutOfRangeException(nameof(unit), unit, null);
        }
    }
}