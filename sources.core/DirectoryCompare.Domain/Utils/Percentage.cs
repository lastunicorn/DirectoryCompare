// DirectoryCompare
// Copyright (C) 2017-2020 Dust in the Wind
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

namespace DustInTheWind.DirectoryCompare.Domain.Utils;

public class Percentage
{
    private readonly DataSize minValue;
    private readonly DataSize size;
    private DataSize underlyingValue;
        
    public float Value { get; private set; }

    public DataSize UnderlyingValue
    {
        get => underlyingValue;
        set
        {
            underlyingValue = value;
            RecalculatePercentageValue();
        }
    }

    public Percentage()
        : this(0, 100)
    {
    }

    public Percentage(DataSize maxValue)
        : this(0, maxValue)
    {
    }

    public Percentage(DataSize minValue, DataSize maxValue)
    {
        if (maxValue <= minValue)
            throw new ArgumentOutOfRangeException(nameof(maxValue));

        this.minValue = minValue;
        size = maxValue - minValue;
    }

    private void RecalculatePercentageValue()
    {
        Value = (float)(UnderlyingValue - minValue) * 100 / size;
    }

    public override string ToString()
    {
        return $"{Value:N2}%";
    }

    public static implicit operator float(Percentage percentage)
    {
        return percentage.Value;
    }

    public static implicit operator double(Percentage percentage)
    {
        return percentage.Value;
    }
}