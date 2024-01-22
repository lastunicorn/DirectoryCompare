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

using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.DataAccess.PotFiles;

public class JsonFileBase<TDocument>
    where TDocument : class
{
    public string FilePath { get; }

    public bool Exists => FilePath != null && File.Exists(FilePath);

    public TDocument Document { get; set; }

    public long Size => new FileInfo(FilePath).Length;

    public bool IsValid => Document != null;

    protected JsonFileBase(string snapshotFilePath)
    {
        FilePath = snapshotFilePath ?? throw new ArgumentNullException(nameof(snapshotFilePath));
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
        if (!Exists)
        {
            Document = null;
            return false;
        }

        try
        {
            Stream stream = File.OpenRead(FilePath);

            using StreamReader streamReader = new(stream);
            using JsonTextReader jsonTextReader = new(streamReader);
            jsonTextReader.MaxDepth = 256;

            JsonSerializer serializer = new();
            Document = (TDocument)serializer.Deserialize(jsonTextReader, typeof(TDocument));

            return Document != null;
        }
        catch
        {
            return false;
        }
    }

    public void Save()
    {
        Stream stream = File.Create(FilePath);

        using StreamWriter streamWriter = new(stream);
        using JsonTextWriter jsonTextWriter = new(streamWriter);
        jsonTextWriter.Formatting = Formatting.Indented;

        JsonSerializer serializer = new();
        serializer.Serialize(jsonTextWriter, Document);
    }

    protected JsonTextReader OpenReader()
    {
        if (!File.Exists(FilePath))
            throw new Exception($"File {FilePath} does not exist.");

        Stream stream = File.OpenRead(FilePath);

        StreamReader streamReader = new(stream);
        return new JsonTextReader(streamReader);
    }

    protected JsonTextWriter OpenWriter()
    {
        string directoryPath = Path.GetDirectoryName(FilePath);
        Directory.CreateDirectory(directoryPath);

        Stream stream = File.Create(FilePath);

        StreamWriter streamWriter = new(stream);
        JsonTextWriter jsonTextWriter = new(streamWriter);
        jsonTextWriter.Formatting = Formatting.Indented;

        return jsonTextWriter;
    }

    public void Delete()
    {
        File.Delete(FilePath);
    }
}