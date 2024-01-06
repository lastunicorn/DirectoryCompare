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

namespace DustInTheWind.DirectoryCompare.DataAccess.PotFiles.SnapshotFileModel;

public class JDirectoryReader : JReader
{
    public JDirectoryFieldType CurrentPropertyType { get; private set; } = JDirectoryFieldType.None;

    public JDirectoryReader(JsonTextReader jsonTextReader)
        : base(jsonTextReader)
    {
    }

    public JDirectoryFieldType MoveToNext()
    {
        switch (State)
        {
            case JReaderState.New:
                State = JReaderState.InProgress;
                break;

            case JReaderState.InProgress:
                break;

            case JReaderState.Finished:
                throw new Exception("The reader already finished reading the json object.");

            default:
                throw new Exception("Invalid reader state.");
        }

        try
        {
            bool success = MoveToNextProperty();

            CurrentPropertyType = success
                ? JsonTextReader.Value switch
                {
                    "n" => JDirectoryFieldType.DirectoryName,
                    "f" => JDirectoryFieldType.FileCollection,
                    "d" => JDirectoryFieldType.SubDirectoryCollection,
                    _ => throw new Exception("Invalid field in directory object.")
                }
                : JDirectoryFieldType.None;

            return CurrentPropertyType;
        }
        catch
        {
            CurrentPropertyType = JDirectoryFieldType.None;
            throw;
        }
    }

    public string ReadName()
    {
        if (CurrentPropertyType != JDirectoryFieldType.DirectoryName)
            throw new Exception("Current property is not the directory name.");

        return JsonTextReader.ReadAsString();
    }

    public IEnumerable<JFileReader> ReadFiles()
    {
        if (CurrentPropertyType != JDirectoryFieldType.FileCollection)
            throw new Exception("Current property is not the file collection.");

        return ReadFilesCollection();
    }

    public IEnumerable<JDirectoryReader> ReadSubDirectories()
    {
        if (CurrentPropertyType != JDirectoryFieldType.SubDirectoryCollection)
            throw new Exception("Current property is not the sub-directory collection.");

        return ReadDirectoriesCollection();
    }
}