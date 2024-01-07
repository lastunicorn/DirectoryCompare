// Directory Compare
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

using System.ComponentModel;

namespace DustInTheWind.Clindy.Applications.PresentFilePreview;

internal class FileExtensions
{
    private readonly Dictionary<string, FileType> fileTypesByExtension = new();

    public void Add(FileType fileType, IEnumerable<string> fileExtensions)
    {
        if (!Enum.IsDefined(typeof(FileType), fileType))
            throw new InvalidEnumArgumentException(nameof(fileType), (int)fileType, typeof(FileType));

        if (fileExtensions == null)
            return;

        IEnumerable<string> cleanedExtensions = fileExtensions
            .Where(x => !string.IsNullOrEmpty(x))
            .Select(x => x.TrimStart('.').ToLower())
            .Distinct();

        foreach (string fileExtension in cleanedExtensions)
            fileTypesByExtension.Add(fileExtension, fileType);
    }

    public FileType FindFileType(string filePath)
    {
        if (filePath == null) throw new ArgumentNullException(nameof(filePath));

        string fileExtension = Path.GetExtension(filePath)
            .TrimStart('.')
            .ToLower();

        bool success = fileTypesByExtension.TryGetValue(fileExtension, out FileType fileType);

        return success
            ? fileType
            : FileType.Unknown;
    }
}