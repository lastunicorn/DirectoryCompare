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

using DustInTheWind.Clindy.Applications.PresentDuplicates;
using DustInTheWind.DirectoryCompare.DataStructures;

namespace DustInTheWind.Clindy.Presentation.ViewModels;

public class DuplicateGroupListItem
{
    public DuplicateGroup DuplicateGroup { get; }

    public string FirstFileName
    {
        get
        {
            List<string> filePaths = DuplicateGroup.FilePaths;
            string firstFilePath = filePaths.FirstOrDefault(x => !string.IsNullOrEmpty(x));

            return firstFilePath == null
                ? "<no name>"
                : Path.GetFileName(firstFilePath);
        }
    }

    public int FileCount => DuplicateGroup.FilePaths.Count;
    
    public string FileSize => DuplicateGroup.FileSize.ToString("simple");

    public DuplicateGroupListItem(DuplicateGroup duplicateGroup)
    {
        DuplicateGroup = duplicateGroup;
    }

    public DuplicateGroupListItem(FileGroup fileGroup)
    {
        DuplicateGroup = new DuplicateGroup
        {
            FileSize = fileGroup.FileSize,
            FileHash = fileGroup.FileHash,
            FilePaths = fileGroup.FilePaths
        };
    }

    public override string ToString()
    {
        List<string> filePaths = DuplicateGroup.FilePaths;

        string firstFilePath = filePaths.FirstOrDefault(x => !string.IsNullOrEmpty(x));
        string fileName = firstFilePath == null
            ? "<no name>"
            : Path.GetFileName(firstFilePath);

        int fileCount = filePaths.Count;

        DataSize fileSize = DuplicateGroup.FileSize;

        return $"{fileName} ({fileCount}) - {fileSize}";
    }
}