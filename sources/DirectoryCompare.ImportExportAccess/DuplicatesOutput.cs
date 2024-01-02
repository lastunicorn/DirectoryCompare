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

using DustInTheWind.DirectoryCompare.Ports.ImportExportAccess;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.ImportExportAccess;

internal class DuplicatesOutput : IDuplicatesOutput
{
    private readonly string path;
    private JsonTextWriter jsonTextWriter;
    private DuplicateFileState state;

    public DuplicatesOutput(string path)
    {
        this.path = path ?? throw new ArgumentNullException(nameof(path));
    }

    public void Open()
    {
        FileStreamOptions fileStreamOptions = new()
        {
            Mode = FileMode.CreateNew,
            Access = FileAccess.Write
        };
        StreamWriter streamWriter = new(path, fileStreamOptions);
        jsonTextWriter = new JsonTextWriter(streamWriter);
        jsonTextWriter.Formatting = Formatting.Indented;

        MoveToOpenedState();
    }

    public void WriteHeader(DuplicatesHeader duplicatesHeader)
    {
        MoveToHeaderState();

        jsonTextWriter.WritePropertyName("Left");
        jsonTextWriter.WriteValue(duplicatesHeader.PotNameLeft);

        jsonTextWriter.WritePropertyName("Right");
        jsonTextWriter.WriteValue(duplicatesHeader.PotNameRight);
    }

    public void WriteDuplicate(Duplicate duplicate)
    {
        MoveToDuplicatesState();

        jsonTextWriter.WriteStartObject();

        jsonTextWriter.WritePropertyName("f");
        jsonTextWriter.WriteStartArray();

        foreach (string fullPath in duplicate.FullPaths) 
            jsonTextWriter.WriteValue(fullPath);
        
        jsonTextWriter.WriteEndArray();

        jsonTextWriter.WritePropertyName("s");
        jsonTextWriter.WriteValue(duplicate.Size);

        jsonTextWriter.WritePropertyName("h");
        jsonTextWriter.WriteValue(duplicate.Hash);

        jsonTextWriter.WriteEndObject();
    }

    public void Close()
    {
        MoveToClosedState();

        jsonTextWriter.Flush();
        jsonTextWriter.Close();
    }

    private void MoveToOpenedState()
    {
        switch (state)
        {
            case DuplicateFileState.None:
                jsonTextWriter.WriteStartObject();
                state = DuplicateFileState.Opened;
                break;

            case DuplicateFileState.Opened:
                break;

            case DuplicateFileState.Header:
            case DuplicateFileState.Duplicates:
            case DuplicateFileState.Closed:
            default:
                throw new InvalidFileStateException(state.ToString());
        }
    }

    private void MoveToHeaderState()
    {
        switch (state)
        {
            case DuplicateFileState.Opened:
                state = DuplicateFileState.Header;
                break;

            case DuplicateFileState.Header:
                break;

            case DuplicateFileState.None:
            case DuplicateFileState.Duplicates:
            case DuplicateFileState.Closed:
            default:
                throw new InvalidFileStateException(state.ToString());
        }
    }

    private void MoveToDuplicatesState()
    {
        switch (state)
        {
            case DuplicateFileState.Header:
                jsonTextWriter.WritePropertyName("Duplicates");
                jsonTextWriter.WriteStartArray();
                state = DuplicateFileState.Duplicates;
                break;

            case DuplicateFileState.Duplicates:
                break;

            case DuplicateFileState.None:
            case DuplicateFileState.Opened:
            case DuplicateFileState.Closed:
            default:
                throw new InvalidFileStateException(state.ToString());
        }
    }

    private void MoveToClosedState()
    {
        switch (state)
        {
            case DuplicateFileState.Duplicates:
                jsonTextWriter.WriteEndArray();
                jsonTextWriter.WriteEndObject();
                state = DuplicateFileState.Closed;
                break;

            case DuplicateFileState.Closed:
                break;

            case DuplicateFileState.None:
                state = DuplicateFileState.Closed;
                break;
            
            case DuplicateFileState.Opened:
                jsonTextWriter.WriteEndObject();
                state = DuplicateFileState.Closed;
                break;
            
            case DuplicateFileState.Header:
                jsonTextWriter.WriteEndObject();
                state = DuplicateFileState.Closed;
                break;
            
            default:
                throw new InvalidFileStateException(state.ToString());
        }
    }
}