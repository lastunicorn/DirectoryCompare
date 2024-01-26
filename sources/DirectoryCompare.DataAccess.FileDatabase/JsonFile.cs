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

using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.DataAccess.FileDatabase;

public class JsonFile<TDocument>
    where TDocument : class
{
    private TDocument document;
    private bool isLoaded;

    private string FilePath => string.IsNullOrEmpty(RootPath)
        ? FileName
        : Path.Combine(RootPath, FileName);

    public string FileName { get; }

    public string RootPath { get; set; }

    public bool Exists => FilePath != null && File.Exists(FilePath);

    public long Size => new FileInfo(FilePath).Length;

    public bool IsValid
    {
        get
        {
            TDocument document = Read();
            return document != null;
        }
    }

    protected JsonFile(string fileName)
    {
        FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
    }

    public TDocument Read(bool force = false)
    {
        if (!isLoaded || force)
            OpenInternal();

        return document;
    }

    private void OpenInternal()
    {
        if (!Exists)
            return;

        Stream stream = File.OpenRead(FilePath);

        using StreamReader streamReader = new(stream);
        using JsonTextReader jsonTextReader = new(streamReader);
        jsonTextReader.MaxDepth = 256;

        JsonSerializer serializer = new();
        document = (TDocument)serializer.Deserialize(jsonTextReader, typeof(TDocument));
        isLoaded = true;
    }

    public void SaveChanges()
    {
        if (!isLoaded)
            return;

        WriteInternal();
    }

    public void SaveNew(TDocument document)
    {
        this.document = document ?? throw new ArgumentNullException(nameof(document));

        WriteInternal();
    }

    private void WriteInternal()
    {
        Stream stream = File.Create(FilePath);

        using StreamWriter streamWriter = new(stream);
        using JsonTextWriter jsonTextWriter = new(streamWriter);
        jsonTextWriter.Formatting = Formatting.Indented;

        JsonSerializer serializer = new();
        serializer.Serialize(jsonTextWriter, document);

        isLoaded = true;
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