// DirectoryCompare
// Copyright (C) 2017-2019 Dust in the Wind
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

using System;
using DustInTheWind.DirectoryCompare.Entities;

namespace DustInTheWind.DirectoryCompare.Comparison
{
    public struct ItemComparison : IEquatable<ItemComparison>
    {
        public string RootPath { get; set; }

        public HItem Item1 { get; set; }

        public HItem Item2 { get; set; }

        public string FullName1 => (RootPath ?? string.Empty) + Item1?.Name;

        public string FullName2 => (RootPath ?? string.Empty) + Item2?.Name;

        public override string ToString()
        {
            return $"{RootPath} - {Item1} - {Item2}";
        }

        public bool Equals(ItemComparison other)
        {
            return string.Equals(RootPath, other.RootPath) &&
                   Equals(Item1, other.Item1) &&
                   Equals(Item2, other.Item2);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ItemComparison && Equals((ItemComparison)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (RootPath != null ? RootPath.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Item1 != null ? Item1.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Item2 != null ? Item2.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}