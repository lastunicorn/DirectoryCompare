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
    /// Indicates whether the current object is equal to another object of the <see cref="DataSize"/> type.
    /// </summary>
    /// 
    /// <param name="other">An <see cref="DataSize"/> object to compare with this object.</param>
    /// 
    /// <returns>
    /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter;
    /// otherwise, <see langword="false" />.
    /// </returns>
    public bool Equals(DataSize other)
    {
        return value.Equals(other.value);
    }

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// 
    /// <param name="obj">The object to compare with the current instance.</param>
    /// 
    /// <returns>
    /// <see langword="true" /> if <paramref name="obj" /> and this instance are the same type (<see cref="DataSize"/>)
    /// and represent the same value; otherwise, <see langword="false" />.
    /// </returns>
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        return obj is DataSize other && Equals(other);
    }

    /// <summary>
    /// Returns the hash code for this instance, calculated based on the underlying value.
    /// </summary>
    /// 
    /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
    public override int GetHashCode()
    {
        return value.GetHashCode();
    }

    /// <summary>
    /// Compares the current instance with another object of the same type (<see cref="DataSize"/> and returns an
    /// integer that indicates whether the current instance precedes, follows, or occurs in the same position in the
    /// sort order as the other object.
    /// </summary>
    /// 
    /// <param name="other">An object to compare with this instance.</param>
    /// 
    /// <returns>
    /// A value that indicates the relative order of the objects being compared. The return value has these meanings:
    /// <list type="table">
    /// <listheader><term> Value</term><description> Meaning</description></listheader>
    /// <item><term> Less than zero</term><description> This instance precedes <paramref name="other" /> in the sort order.</description></item>
    /// <item><term> Zero</term><description> This instance occurs in the same position in the sort order as <paramref name="other" />.</description></item>
    /// <item><term> Greater than zero</term><description> This instance follows <paramref name="other" /> in the sort order.</description></item>
    /// </list>
    /// </returns>
    public int CompareTo(DataSize other)
    {
        return value.CompareTo(other.value);
    }
}