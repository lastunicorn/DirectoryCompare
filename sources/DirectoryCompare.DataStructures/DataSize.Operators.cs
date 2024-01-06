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
    #region Operator *
    
    public static DataSize operator *(DataSize dataSize1, ulong dataSize2)
    {
        return new DataSize(dataSize1.Value * dataSize2);
    }

    public static DataSize operator *(ulong dataSize1, DataSize dataSize2)
    {
        return new DataSize(dataSize1 * dataSize2.Value);
    }

    #endregion

    #region Operator -

    public static DataSize operator -(DataSize dataSize1, ulong dataSize2)
    {
        return new DataSize(dataSize1.Value - dataSize2);
    }

    public static DataSize operator -(ulong dataSize1, DataSize dataSize2)
    {
        return new DataSize(dataSize2.Value - dataSize1);
    }

    public static DataSize operator -(DataSize dataSize1, DataSize dataSize2)
    {
        return new DataSize(dataSize1.Value - dataSize2.Value);
    }

    #endregion

    #region Operator +

    public static DataSize operator +(DataSize dataSize1, ulong dataSize2)
    {
        return new DataSize(dataSize1.Value + dataSize2);
    }

    public static DataSize operator +(ulong dataSize1, DataSize dataSize2)
    {
        return new DataSize(dataSize2.Value + dataSize1);
    }

    public static DataSize operator +(DataSize dataSize1, DataSize dataSize2)
    {
        return new DataSize(dataSize1.Value + dataSize2.Value);
    }

    #endregion

    #region Operator <

    public static bool operator <(DataSize dataSize1, DataSize dataSize2)
    {
        return dataSize1.Value < dataSize2.Value;
    }

    public static bool operator <(DataSize dataSize1, ulong dataSize2)
    {
        return dataSize1.Value < dataSize2;
    }

    public static bool operator <(DataSize dataSize1, int dataSize2)
    {
        if (dataSize2 < 0)
            return false;

        return dataSize1.Value < (ulong)dataSize2;
    }

    #endregion

    #region Operator >

    public static bool operator >(DataSize dataSize1, DataSize dataSize2)
    {
        return dataSize1.Value > dataSize2.Value;
    }

    public static bool operator >(DataSize dataSize1, ulong dataSize2)
    {
        return dataSize1.Value > dataSize2;
    }

    public static bool operator >(DataSize dataSize1, int dataSize2)
    {
        if (dataSize2 < 0)
            return true;

        return dataSize1.Value > (ulong)dataSize2;
    }

    #endregion

    #region Operator <=

    public static bool operator <=(DataSize dataSize1, DataSize dataSize2)
    {
        return dataSize1.Value <= dataSize2.Value;
    }

    public static bool operator <=(DataSize dataSize1, ulong dataSize2)
    {
        return dataSize1.Value <= dataSize2;
    }

    public static bool operator <=(DataSize dataSize1, int dataSize2)
    {
        if (dataSize2 < 0)
            return false;

        return dataSize1.Value <= (ulong)dataSize2;
    }

    #endregion

    #region Operator >=

    public static bool operator >=(DataSize dataSize1, DataSize dataSize2)
    {
        return dataSize1.Value >= dataSize2.Value;
    }

    public static bool operator >=(DataSize dataSize1, ulong dataSize2)
    {
        return dataSize1.Value >= dataSize2;
    }

    public static bool operator >=(DataSize dataSize1, int dataSize2)
    {
        if (dataSize2 < 0)
            return true;

        return dataSize1.Value >= (ulong)dataSize2;
    }

    #endregion
}