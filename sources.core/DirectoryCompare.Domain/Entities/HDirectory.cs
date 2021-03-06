﻿// DirectoryCompare
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DustInTheWind.DirectoryCompare.Domain.Entities
{
    public class HDirectory : HItem, IEquatable<HDirectory>, IEnumerable<HItem>
    {
        public HItemCollection<HDirectory> Directories { get; }

        public HItemCollection<HFile> Files { get; }

        public HDirectory()
        {
            Directories = new HItemCollection<HDirectory>(this);
            Files = new HItemCollection<HFile>(this);
        }

        public HDirectory(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Directories = new HItemCollection<HDirectory>(this);
            Files = new HItemCollection<HFile>(this);
        }

        public IEnumerable<HFile> EnumerateFiles(string path, BlackList blackList = null)
        {
            IEnumerable<HFile> filesQuery = EnumerateFiles(blackList);

            if (path != null)
                filesQuery = filesQuery.Where(x =>
                {
                    if (!path.StartsWith(Path.DirectorySeparatorChar))
                        path = Path.DirectorySeparatorChar + path;

                    return x.GetPath().StartsWith(path);
                });

            return filesQuery;
        }

        public IEnumerable<HFile> EnumerateFiles(BlackList blackList = null)
        {
            if (Files != null)
            {
                foreach (HFile file in Files)
                {
                    if (blackList != null && blackList.MatchPath(file))
                        continue;

                    yield return file;
                }
            }

            if (Directories != null)
            {
                foreach (HDirectory xSubDirectory in Directories)
                {
                    if (blackList != null && blackList.MatchPath(xSubDirectory))
                        continue;

                    foreach (HFile file in xSubDirectory.EnumerateFiles())
                    {
                        if (blackList != null && blackList.MatchPath(file))
                            continue;

                        yield return file;
                    }
                }
            }
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
            if (obj.GetType() != GetType()) return false;

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