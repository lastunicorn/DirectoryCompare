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

using System.Collections.ObjectModel;

namespace DustInTheWind.DirectoryCompare.DataStructures;

public class FileHashCollection : Collection<FileHash>
{
    public FileHashCollection()
    {
    }

    public FileHashCollection(IList<FileHash> items)
        : base(items)
    {
    }

    public FileHashCollection(IEnumerable<FileHash> items)
        : base(items.ToArray())
    {
    }

    public FileHashCollection(IEnumerable<string> items)
        : base(items.Select(FileHash.Parse).ToArray())
    {
    }

    public void AddRange(IEnumerable<FileHash> items)
    {
        foreach (FileHash item in items)
            Items.Add(item);
    }
}