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

using System.Text.RegularExpressions;

namespace DustInTheWind.DirectoryCompare.DataStructures;

public readonly partial struct DataSize
{
    private static readonly Regex Regex = new(@"^\s*(\d+\.?\d*)\s*(b|kib|mib|gib|tib|pib|kb|mb|gb|tb|pb)*\s*$", RegexOptions.IgnoreCase);

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

        text = text.Trim();
        
        bool isByte = text.Equals("b", StringComparison.InvariantCultureIgnoreCase);
        if (isByte)
            return DataSizeUnit.Byte;

        // ---

        bool isKibibyte = text.Equals("kib", StringComparison.InvariantCultureIgnoreCase);
        if (isKibibyte)
            return DataSizeUnit.Kibibyte;

        bool isMebibyte = text.Equals("mib", StringComparison.InvariantCultureIgnoreCase);
        if (isMebibyte)
            return DataSizeUnit.Mebibyte;

        bool isGibibyte = text.Equals("gib", StringComparison.InvariantCultureIgnoreCase);
        if (isGibibyte)
            return DataSizeUnit.Gibibyte;

        bool isTebibyte = text.Equals("tib", StringComparison.InvariantCultureIgnoreCase);
        if (isTebibyte)
            return DataSizeUnit.Tebibyte;

        bool isPebibyte = text.Equals("pib", StringComparison.InvariantCultureIgnoreCase);
        if (isPebibyte)
            return DataSizeUnit.Pebibyte;
        
        // ---
        
        bool isKilobyte = text.Equals("kb", StringComparison.InvariantCultureIgnoreCase);
        if (isKilobyte)
            return DataSizeUnit.Kilobyte;
        
        bool isMegabyte = text.Equals("mb", StringComparison.InvariantCultureIgnoreCase);
        if (isMegabyte)
            return DataSizeUnit.Megabyte;
        
        bool isGigabyte = text.Equals("gb", StringComparison.InvariantCultureIgnoreCase);
        if (isGigabyte)
            return DataSizeUnit.Gigabyte;
        
        bool isTerabyte = text.Equals("tb", StringComparison.InvariantCultureIgnoreCase);
        if (isTerabyte)
            return DataSizeUnit.Terabyte;
        
        bool isPetabyte = text.Equals("pb", StringComparison.InvariantCultureIgnoreCase);
        if (isPetabyte)
            return DataSizeUnit.Petabyte;

        // ---

        return DataSizeUnit.Unknown;
    }
}