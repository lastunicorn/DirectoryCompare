// VeloCity
// Copyright (C) 2022-2023 Dust in the Wind
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

namespace DustInTheWind.DirectoryCompare.JFiles;

public class JsonFileBase<TContent>
    where TContent : class
{
    public string FilePath { get; }

    public bool Exists => FilePath != null && File.Exists(FilePath);

    public TContent Content { get; set; }

    public bool IsValid => Content != null;

    public JsonFileBase(string snapshotFilePath)
    {
        this.FilePath = snapshotFilePath ?? throw new ArgumentNullException(nameof(snapshotFilePath));
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
            Content = null;
            return false;
        }

        using StreamReader streamReader = File.OpenText(FilePath);
        using JsonTextReader jsonTextReader = new(streamReader);

        JsonSerializer serializer = new();
        Content = (TContent)serializer.Deserialize(jsonTextReader, typeof(TContent));

        return Content != null;
    }

    public void Save()
    {
        using FileStream stream = File.OpenWrite(FilePath);
        using StreamWriter streamWriter = new(stream);
        using JsonTextWriter jsonTextWriter = new(streamWriter);

        JsonSerializer serializer = new();
        serializer.Serialize(jsonTextWriter, Content);
    }

    public JsonTextReader OpenReader()
    {
        if (!File.Exists(FilePath))
            throw new Exception($"File {FilePath} does not exist.");

        StreamReader streamReader = new(FilePath);
        return new JsonTextReader(streamReader);
    }

    public JsonTextWriter OpenWriter()
    {
        string directoryPath = Path.GetDirectoryName(FilePath);
        Directory.CreateDirectory(directoryPath);

        StreamWriter streamWriter = new(FilePath);
        JsonTextWriter jsonTextWriter = new(streamWriter);
        jsonTextWriter.Formatting = Formatting.Indented;

        return jsonTextWriter;
    }

    public void Delete()
    {
        File.Delete(FilePath);
    }
}