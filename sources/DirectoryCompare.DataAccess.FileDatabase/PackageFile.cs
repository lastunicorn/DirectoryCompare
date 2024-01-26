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

using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.DataAccess.FileDatabase;

public abstract class PackageFile
{
    public string FilePath { get; }

    public bool Exists => FilePath != null && File.Exists(FilePath);

    public long Size => new FileInfo(FilePath).Length;

    protected List<PackageDocument> Documents { get; } = new();

    protected PackageFile(string filePath)
    {
        FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
    }

    public bool TryOpen()
    {
        try
        {
            return Open();
        }
        catch
        {
            return false;
        }
    }

    public bool Open()
    {
        if (!File.Exists(FilePath))
            throw new Exception($"File {FilePath} does not exist.");

        FileStream fileStream = File.OpenRead(FilePath);
        ZipInputStream zipInputStream = new ZipInputStream(fileStream);

        while (true)
        {
            ZipEntry zipEntry = zipInputStream.GetNextEntry();

            if (zipEntry == null)
                break;

            PackageDocument packageDocument = Documents
                .FirstOrDefault(x => x.Name == zipEntry.Name);

            if (packageDocument == null)
                continue;

            packageDocument.OpenFrom(zipInputStream);
        }

        return Documents.All(x => x.Content != null);
    }

    public void Save()
    {
        string directoryPath = Path.GetDirectoryName(FilePath);
        Directory.CreateDirectory(directoryPath);

        using Stream stream = File.Create(FilePath);
        using ZipOutputStream zipOutputStream = new(stream);

        foreach (PackageDocument packageDocument in Documents)
        {
            packageDocument.SaveInto(zipOutputStream);
        }
    }

    public JsonTextWriter OpenWriter(string documentName)
    {
        string directoryPath = Path.GetDirectoryName(FilePath);
        Directory.CreateDirectory(directoryPath);

        Stream stream = File.Create(FilePath);
        ZipOutputStream zipOutputStream = new(stream);

        ZipEntry zipEntry = new(documentName);
        zipOutputStream.PutNextEntry(zipEntry);

        StreamWriter streamWriter = new(zipOutputStream);
        JsonTextWriter jsonTextWriter = new(streamWriter);
        jsonTextWriter.Formatting = Formatting.Indented;

        return jsonTextWriter;
    }

    public void Delete()
    {
        File.Delete(FilePath);
    }
}