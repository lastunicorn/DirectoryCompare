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
using System.Collections;
using System.Collections.Generic;

namespace DustInTheWind.DirectoryCompare.Entities
{
    public class HDirectory : HItem, IEquatable<HDirectory>, IEnumerable<HItem>
    {
        public List<HDirectory> Directories { get; } = new List<HDirectory>();

        public List<HFile> Files { get; } = new List<HFile>();

        public HDirectory()
        {
        }

        public HDirectory(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public IEnumerator<HItem> GetEnumerator()
        {
            if (Files != null)
                foreach (HFile file in Files)
                    yield return file;

            if (Directories != null)
                foreach (HDirectory directory in Directories)
                    yield return directory;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Equals(HDirectory other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return base.Equals(other) &&
                   Equals(Directories, other.Directories) &&
                   Equals(Files, other.Files);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;

            return Equals((HDirectory)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();

                hashCode = (hashCode * 397) ^ (Directories != null ? Directories.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Files != null ? Files.GetHashCode() : 0);

                return hashCode;
            }
        }
    }
}