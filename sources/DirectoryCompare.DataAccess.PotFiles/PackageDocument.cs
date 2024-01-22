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

namespace DustInTheWind.DirectoryCompare.DataAccess.PotFiles;

public class PackageDocument
{
    public string Name { get; set; }

    public Type Type { get; set; }

    public object Content { get; private set; }
    
    internal PackageFile Parent { get; set; }

    internal void OpenFrom(ZipInputStream zipInputStream)
    {
        using StreamReader streamReader = new(zipInputStream);
        using JsonTextReader jsonTextReader = new(streamReader);
        jsonTextReader.MaxDepth = 256;

        JsonSerializer serializer = new();
        Content = serializer.Deserialize(jsonTextReader, Type);
    }

    internal void SaveInto(ZipOutputStream zipOutputStream)
    {
        ZipEntry zipEntry = new(Name);
        zipOutputStream.PutNextEntry(zipEntry);
            
        using StreamWriter streamWriter = new(zipOutputStream);
        using JsonTextWriter jsonTextWriter = new(streamWriter);
        jsonTextWriter.Formatting = Formatting.Indented;

        JsonSerializer serializer = new();
        serializer.Serialize(jsonTextWriter, Content);
    }
    
    protected JsonTextWriter OpenWriter()
    {
        string filePath = Parent.FilePath;
        
        string directoryPath = Path.GetDirectoryName(filePath);
        Directory.CreateDirectory(directoryPath);

        Stream stream = File.Create(filePath);
        ZipOutputStream zipOutputStream = new(stream);
        
        ZipEntry zipEntry = new(Name);
        zipOutputStream.PutNextEntry(zipEntry);
            
        StreamWriter streamWriter = new(zipOutputStream);
        JsonTextWriter jsonTextWriter = new(streamWriter);
        jsonTextWriter.Formatting = Formatting.Indented;

        return jsonTextWriter;
    }
}