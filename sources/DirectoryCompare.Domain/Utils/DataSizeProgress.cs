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

using DustInTheWind.DirectoryCompare.DataStructures;

namespace DustInTheWind.DirectoryCompare.Domain.Utils;

public class DataSizeProgress
{
    private DataSize value;

    public DataSize MinValue { get; }

    public DataSize MaxValue { get; }

    public DataSize Size { get; }

    public float Percentage { get; private set; }

    public DataSize Value
    {
        get => value;
        set
        {
            this.value = value;
            RecalculatePercentageValue();
        }
    }

    public DataSizeProgress(DataSize maxValue)
        : this(DataSize.Zero, maxValue)
    {
    }

    public DataSizeProgress(DataSize minValue, DataSize maxValue)
    {
        if (maxValue < minValue)
            throw new ArgumentOutOfRangeException(nameof(maxValue));

        MinValue = minValue;
        MaxValue = maxValue;
        Size = maxValue - minValue;
        
        RecalculatePercentageValue();
    }

    private void RecalculatePercentageValue()
    {
        Percentage = Size.IsZero
            ? 100
            : (float)(Value - MinValue) * 100 / Size;
    }

    public override string ToString()
    {
        return $"{Percentage:N2}%";
    }

    public static implicit operator float(DataSizeProgress progress)
    {
        return progress.Percentage;
    }

    public static implicit operator double(DataSizeProgress progress)
    {
        return progress.Percentage;
    }
}