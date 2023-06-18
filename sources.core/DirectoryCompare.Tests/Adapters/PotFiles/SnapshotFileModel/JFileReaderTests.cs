// DirectoryCompare
// Copyright (C) 2017-2020 Dust in the Wind
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

using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Domain.Utils;
using DustInTheWind.DirectoryCompare.JFiles.SnapshotFileModel;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace DustInTheWind.DirectoryCompare.Tests.Adapters.PotFiles.SnapshotFileModel;

public class JFileReaderTests
{
    private const string Json = @"{
  ""n"": ""filename.extension"",
  ""s"": ""15"",
  ""m"": ""2011-12-21T11:14:16Z"",
  ""h"": ""DQZQ""
}";

    [Fact]
    public void HavingJsonNodeRepresentingFullFile_WhenJFileIsParsed_FileNameIsReadCorrectly()
    {
        JFileFields fields = ReadAllFieldsFrom(Json);

        fields.FileName.Should().Be("filename.extension");
    }

    [Fact]
    public void HavingJsonNodeRepresentingFullFile_WhenJFileIsParsed_FileSizeIsReadCorrectly()
    {
        JFileFields fields = ReadAllFieldsFrom(Json);

        DataSize expected = DataSize.FromBytes(15);
        fields.FileSize.Should().Be(expected);
    }

    [Fact]
    public void HavingJsonNodeRepresentingFullFile_WhenJFileIsParsed_LastModifiedTimeIsReadCorrectly()
    {
        JFileFields fields = ReadAllFieldsFrom(Json);

        DateTime expected = new DateTime(2011, 12, 21, 11, 14, 16);
        fields.LastModifiedTime.Should().Be(expected);
    }

    [Fact]
    public void HavingJsonNodeRepresentingFullFile_WhenJFileIsParsed_FileHashIsReadCorrectly()
    {
        JFileFields fields = ReadAllFieldsFrom(Json);

        FileHash expected = new FileHash(new byte[] { 13, 6, 80 });
        fields.FileHash.Should().Be(expected);
    }

    private static JFileFields ReadAllFieldsFrom(string json)
    {
        using StringReader stringReader = new(json);
        using JsonTextReader jsonTextReader = new(stringReader);
        jsonTextReader.Read();

        JFileFields fields = new();

        JFileReader jFileReader = new(jsonTextReader);

        while (true)
        {
            JFileFieldType fieldType = jFileReader.MoveToNext();

            if (fieldType == JFileFieldType.None)
                break;

            switch (fieldType)
            {
                case JFileFieldType.None:
                    break;

                case JFileFieldType.FileName:
                    fields.FileName = jFileReader.ReadName();
                    break;

                case JFileFieldType.FileSize:
                    fields.FileSize = jFileReader.ReadSize();
                    break;

                case JFileFieldType.LastModifiedTime:
                    fields.LastModifiedTime = jFileReader.ReadLastModifiedTime();
                    break;

                case JFileFieldType.Hash:
                    fields.FileHash = jFileReader.ReadHash();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return fields;
    }
}