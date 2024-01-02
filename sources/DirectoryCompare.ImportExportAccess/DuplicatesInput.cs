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

using DustInTheWind.DirectoryCompare.DataStructures;
using DustInTheWind.DirectoryCompare.Ports.ImportExportAccess;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.ImportExportAccess;

public class DuplicatesInput : IDuplicatesInput
{
    private readonly string path;

    private DuplicatesDocument duplicatesDocument;

    public DuplicatesInput(string path)
    {
        this.path = path ?? throw new ArgumentNullException(nameof(path));
    }

    public void Open()
    {
        JsonSerializer serializer = new();

        using StreamReader streamReader = new(path);
        using JsonTextReader jsonTextReader = new(streamReader);

        duplicatesDocument = serializer.Deserialize<DuplicatesDocument>(jsonTextReader);
    }

    public DuplicatesHeader GetHeader()
    {
        return new DuplicatesHeader
        {
            PotNameLeft = duplicatesDocument.Left,
            PotNameRight = duplicatesDocument.Right
        };
    }

    public IEnumerable<FileDuplicateGroup> EnumerateDuplicates()
    {
        return duplicatesDocument.Duplicates
            .Select(x => new FileDuplicateGroup
            {
                FilePaths = x.FilePaths,
                FileSize = x.FileSize,
                FileHash = FileHash.Parse(x.FileHash)
            });
    }
}