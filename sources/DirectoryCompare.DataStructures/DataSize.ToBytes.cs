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
    /// Returns another <see cref="DataSize"/> object with the value converted in Bytes.
    /// </summary>
    public static ulong ToBytes(ulong value, DataSizeUnit unit)
    {
        switch (unit)
        {
            case DataSizeUnit.Unknown:
            case DataSizeUnit.Byte:
                return value;

            // ---
            
            case DataSizeUnit.Kibibyte:
                return value * OneKibibyteValue;

            case DataSizeUnit.Mebibyte:
                return value * OneMebibyteValue;

            case DataSizeUnit.Gibibyte:
                return value * OneGibibyteValue;

            case DataSizeUnit.Tebibyte:
                return value * OneTebibyteValue;

            case DataSizeUnit.Pebibyte:
                return value * OnePebibyteValue;

            // ---
            
            case DataSizeUnit.Kilobyte:
                return value * OneKilobyteValue;

            case DataSizeUnit.Megabyte:
                return value * OneMegabyteValue;

            case DataSizeUnit.Gigabyte:
                return value * OneGigabyteValue;

            case DataSizeUnit.Terabyte:
                return value * OneTerabyteValue;

            case DataSizeUnit.Petabyte:
                return value * OnePetabyteValue;

            // ---

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// Returns another <see cref="DataSize"/> object with the value converted in Bytes.
    /// </summary>
    public static ulong ToBytes(double value, DataSizeUnit unit)
    {
        if (value < 0)
            return 0;

        switch (unit)
        {
            case DataSizeUnit.Unknown:
            case DataSizeUnit.Byte:
                return (ulong)Math.Round(value);

            // ---

            case DataSizeUnit.Kibibyte:
                return (ulong)Math.Round(value) * OneKibibyteValue;

            case DataSizeUnit.Mebibyte:
                return (ulong)Math.Round(value) * OneMebibyteValue;

            case DataSizeUnit.Gibibyte:
                return (ulong)Math.Round(value) * OneGibibyteValue;

            case DataSizeUnit.Tebibyte:
                return (ulong)Math.Round(value) * OneTebibyteValue;

            case DataSizeUnit.Pebibyte:
                return (ulong)Math.Round(value) * OnePebibyteValue;

            // ---

            case DataSizeUnit.Kilobyte:
                return (ulong)Math.Round(value) * OneKilobyteValue;

            case DataSizeUnit.Megabyte:
                return (ulong)Math.Round(value) * OneMegabyteValue;

            case DataSizeUnit.Gigabyte:
                return (ulong)Math.Round(value) * OneGigabyteValue;

            case DataSizeUnit.Terabyte:
                return (ulong)Math.Round(value) * OneTerabyteValue;

            case DataSizeUnit.Petabyte:
                return (ulong)Math.Round(value) * OnePetabyteValue;

            // ---

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}