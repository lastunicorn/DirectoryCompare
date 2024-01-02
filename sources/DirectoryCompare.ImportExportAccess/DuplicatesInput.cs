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
    // private JsonTextReader jsonTextWriter;
    // private DuplicateFileState state;
    // private DuplicatesHeader duplicatesHeader;

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

        // StreamReader streamReader = new(path);
        // jsonTextWriter = new JsonTextReader(streamReader);
        //
        // MoveToOpenedState();
    }

    public DuplicatesHeader GetHeader()
    {
        return new DuplicatesHeader
        {
            PotNameLeft = duplicatesDocument.Left,
            PotNameRight = duplicatesDocument.Right
        };

        //return duplicatesHeader ??= ReadHeader();
    }

    // public DuplicatesHeader ReadHeader()
    // {
    //     MoveToHeaderState();
    //
    //     duplicatesHeader = new DuplicatesHeader();
    //
    //     while (jsonTextWriter.Read())
    //     {
    //         if (jsonTextWriter.TokenType == JsonToken.Comment)
    //             continue;
    //
    //         if (jsonTextWriter.TokenType == JsonToken.PropertyName)
    //         {
    //             bool isLeftProperty = string.Equals(jsonTextWriter.Value as string, "Left", StringComparison.InvariantCultureIgnoreCase);
    //             if (isLeftProperty)
    //             {
    //                 duplicatesHeader.PotNameLeft = jsonTextWriter.ReadAsString();
    //                 continue;
    //             }
    //
    //             bool isRightProperty = string.Equals(jsonTextWriter.Value as string, "Right", StringComparison.InvariantCultureIgnoreCase);
    //             if (isRightProperty)
    //             {
    //                 duplicatesHeader.PotNameRight = jsonTextWriter.ReadAsString();
    //                 continue;
    //             }
    //
    //             bool isDuplicatesProperty = string.Equals(jsonTextWriter.Value as string, "Duplicates", StringComparison.InvariantCultureIgnoreCase);
    //             if (isDuplicatesProperty)
    //             {
    //                 state = DuplicateFileState.Duplicates;
    //                 break;
    //             }
    //
    //             throw new Exception("Invalid property detected: " + jsonTextWriter.Value);
    //         }
    //
    //         throw new Exception("Could not read the duplicates json file. Invalid structure.");
    //     }
    //
    //     return duplicatesHeader;
    // }

    public IEnumerable<FileDuplicateGroup> EnumerateDuplicates()
    {
        return duplicatesDocument.Duplicates
            .Select(x => new FileDuplicateGroup
            {
                FilePaths = x.FilePaths,
                FileSize = x.FileSize,
                FileHash = FileHash.Parse(x.FileHash)
            });

        // MoveToDuplicatesState();
        //
        // while (jsonTextWriter.Read())
        // {
        //     if (jsonTextWriter.TokenType == JsonToken.Comment)
        //         continue;
        //
        //     if (jsonTextWriter.TokenType == JsonToken.StartArray)
        //         break;
        //
        //     throw new Exception("Invalid file structure.");
        // }
        //
        // yield return ReadFileDuplicateGroup();
    }

    // private FileDuplicateGroup ReadFileDuplicateGroup()
    // {
    //     while (jsonTextWriter.Read())
    //     {
    //         if (jsonTextWriter.TokenType == JsonToken.Comment)
    //             continue;
    //
    //         if (jsonTextWriter.TokenType == JsonToken.StartObject)
    //             break;
    //
    //         throw new Exception("Invalid file structure.");
    //     }
    //     
    //     
    // }
    //
    // private void Read
    //
    // private void MoveToOpenedState()
    // {
    //     switch (state)
    //     {
    //         case DuplicateFileState.None:
    //             while (jsonTextWriter.Read())
    //             {
    //                 if (jsonTextWriter.TokenType != JsonToken.Comment)
    //                     break;
    //             }
    //
    //             if (jsonTextWriter.TokenType != JsonToken.StartObject)
    //                 throw new Exception("Invalid input file. The file should contain an object as a root token.");
    //
    //             state = DuplicateFileState.Opened;
    //             break;
    //
    //         case DuplicateFileState.Opened:
    //             break;
    //
    //         case DuplicateFileState.Header:
    //         case DuplicateFileState.Duplicates:
    //         case DuplicateFileState.Closed:
    //         default:
    //             throw new InvalidFileStateException(state.ToString());
    //     }
    // }
    //
    // private void MoveToHeaderState()
    // {
    //     switch (state)
    //     {
    //         case DuplicateFileState.Opened:
    //             state = DuplicateFileState.Header;
    //             break;
    //
    //         case DuplicateFileState.Header:
    //             break;
    //
    //         case DuplicateFileState.None:
    //         case DuplicateFileState.Duplicates:
    //         case DuplicateFileState.Closed:
    //         default:
    //             throw new InvalidFileStateException(state.ToString());
    //     }
    // }
    //
    // private void MoveToDuplicatesState()
    // {
    //     switch (state)
    //     {
    //         case DuplicateFileState.Opened:
    //         case DuplicateFileState.Header:
    //             _ = ReadHeader();
    //             break;
    //
    //         case DuplicateFileState.Duplicates:
    //             break;
    //
    //         case DuplicateFileState.None:
    //         case DuplicateFileState.Closed:
    //         default:
    //             throw new InvalidFileStateException(state.ToString());
    //     }
    // }
}