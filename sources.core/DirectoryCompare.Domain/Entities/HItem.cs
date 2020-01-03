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
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DustInTheWind.DirectoryCompare.Domain.Entities
{
    public class HItem : IEquatable<HItem>
    {
        public string Name { get; set; }

        public string Error { get; set; }

        public HItem Parent { get; set; }

        public string GetPath()
        {
            List<string> items = new List<string>();

            HItem item = this;

            while (item != null)
            {
                items.Add(item.Name);
                item = item.Parent;
            }

            IEnumerable<string> reversedItems = ((IEnumerable<string>)items).Reverse();
            return string.Join(Path.DirectorySeparatorChar, reversedItems);
        }

        public string GetOriginalPath()
        {
            List<string> items = new List<string>();

            HItem item = this;
            Snapshot snapshot = null;

            while (item != null)
            {
                items.Add(item.Name);

                if (item.Parent is Snapshot s)
                    snapshot = s;

                item = item.Parent;
            }

            IEnumerable<string> reversedItems = ((IEnumerable<string>)items).Reverse();
            string relativePath = string.Join(Path.DirectorySeparatorChar, reversedItems);

            if (snapshot == null)
                return relativePath;

            relativePath = relativePath.Substring(Path.GetPathRoot(relativePath).Length);
            return Path.Combine(snapshot.OriginalPath, relativePath);
        }

        public override string ToString()
        {
            return Name;
        }

        public bool Equals(HItem other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return string.Equals(Name, other.Name) &&
                   string.Equals(Error, other.Error);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            return Equals((HItem)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^
                       (Error != null ? Error.GetHashCode() : 0);
            }
        }
    }
}