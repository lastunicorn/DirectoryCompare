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
using DustInTheWind.DirectoryCompare.Cli.Application.MiscellaneousArea.FindDuplicates;
using DustInTheWind.DirectoryCompare.DataStructures;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.MiscellaneousCommands.FindDuplicates;

internal class FileDuplicatesViewModel : IEnumerable<FilePairDto>
{
    private readonly FilePairDto[] fileDuplicates;

    public int DuplicateCount => fileDuplicates?.Length ?? 0;

    public DataSize TotalSize => fileDuplicates?.Sum(x => (long)(ulong)x.Size) ?? DataSize.Zero;

    public FileDuplicatesViewModel(FilePairDto[] fileDuplicates)
    {
        this.fileDuplicates = fileDuplicates ?? throw new ArgumentNullException(nameof(fileDuplicates));
    }

    public IEnumerator<FilePairDto> GetEnumerator()
    {
        IEnumerable<FilePairDto> enumerable = fileDuplicates;
        return enumerable.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}