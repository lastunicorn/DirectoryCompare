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

using DustInTheWind.DirectoryCompare.DataAccess.PotFiles.SnapshotFileModel;

namespace DustInTheWind.DirectoryCompare.DataAccess.PotFiles;

public class SnapshotPackage : PackageFile
{
    private readonly SnapshotFilePath snapshotFilePath;

    public SnapshotPackage(string filePath)
        : base(filePath)
    {
        snapshotFilePath = filePath;
        
        Documents.Add(new PackageDocument
        {
            Name = "snapshot.json",
            Type = typeof(JSnapshot)
        });
    }

    public DateTime? CreationTime => snapshotFilePath.CreationTime;

    // private readonly string packageFilePath;
    // private ZipInputStream zipInputStream;
    //
    // public SnapshotPackage(string packageFilePath)
    // {
    //     this.packageFilePath = packageFilePath ?? throw new ArgumentNullException(nameof(packageFilePath));
    // }
    //
    // public void Open()
    // {
    //     if (!File.Exists(packageFilePath))
    //         throw new Exception($"File {packageFilePath} does not exist.");
    //
    //     FileStream fileStream = File.OpenRead(packageFilePath);
    //     zipInputStream = new ZipInputStream(fileStream);
    // }
    //
    // public TDocument GetDocument<TDocument>(string documentName)
    //     where TDocument : class
    // {
    //     while (true)
    //     {
    //         ZipEntry zipEntry = zipInputStream.GetNextEntry();
    //
    //         if (zipEntry == null)
    //             break;
    //
    //         if (zipEntry.Name == documentName) //"snapshot.json"
    //         {
    //             using StreamReader streamReader = new(zipInputStream);
    //             using JsonTextReader jsonTextReader = new(streamReader);
    //             jsonTextReader.MaxDepth = 256;
    //
    //             JsonSerializer serializer = new();
    //             return (TDocument)serializer.Deserialize(jsonTextReader, typeof(TDocument));
    //         }
    //     }
    //
    //     throw new Exception($"Document was not found: '{documentName}'");
    // }
}