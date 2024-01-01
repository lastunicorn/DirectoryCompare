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

using System.Collections;
using DustInTheWind.DirectoryCompare.DataStructures;
using DustInTheWind.DirectoryCompare.Domain.Entities;

namespace DustInTheWind.DirectoryCompare.Domain.Comparison;

public class FileGroup : IEnumerable<HFile>
{
    private readonly HFile[] files;

    public DataSize Size => files.FirstOrDefault()?.Size ?? DataSize.Zero;

    public FileHash Hash => files.FirstOrDefault()?.Hash ?? FileHash.Empty;

    public int Count => files.Length;

    public FileGroup(IEnumerable<HFile> files)
    {
        if (files == null) throw new ArgumentNullException(nameof(files));

        this.files = files.ToArray();
    }

    public IEnumerable<FilePair> EnumeratePairs()
    {
        for (int i = 0; i < files.Length - 1; i++)
        {
            for (int j = i + 1; j < files.Length; j++)
            {
                yield return new FilePair(files[i], files[j]);
            }
        }
    }

    public IEnumerator<HFile> GetEnumerator()
    {
        IEnumerable<HFile> enumerableFiles = files;
        return enumerableFiles.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}