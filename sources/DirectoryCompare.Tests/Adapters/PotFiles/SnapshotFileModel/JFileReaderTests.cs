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

using DustInTheWind.DirectoryCompare.DataAccess.PotFiles.SnapshotFileModel;
using DustInTheWind.DirectoryCompare.DataStructures;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace DustInTheWind.DirectoryCompare.Tests.Adapters.PotFiles.SnapshotFileModel;

public class JFileReaderTests
{
    private readonly JDummyObject jDummyObject;

    public JFileReaderTests()
    {
        string json = EmbeddedResources.GetContent("Data-DummyFile.json");
        JFileReader jFileReader = CreateReader(json);
        jDummyObject = ReadAllFieldsFrom(jFileReader);
    }

    private static JFileReader CreateReader(string json)
    {
        StringReader stringReader = new(json);
        JsonTextReader jsonTextReader = new(stringReader);
        jsonTextReader.Read();

        return new JFileReader(jsonTextReader);
    }

    private static JDummyObject ReadAllFieldsFrom(JFileReader jFileReader)
    {
        JDummyObject fields = new();

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

    [Fact]
    public void HavingJsonNodeRepresentingFullFile_WhenJFileIsParsed_FileNameIsReadCorrectly()
    {
        jDummyObject.FileName.Should().Be("filename.extension");
    }

    [Fact]
    public void HavingJsonNodeRepresentingFullFile_WhenJFileIsParsed_FileSizeIsReadCorrectly()
    {
        DataSize expected = DataSize.FromBytes(15);
        jDummyObject.FileSize.Should().Be(expected);
    }

    [Fact]
    public void HavingJsonNodeRepresentingFullFile_WhenJFileIsParsed_LastModifiedTimeIsReadCorrectly()
    {
        DateTime expected = new(2011, 12, 21, 11, 14, 16);
        jDummyObject.LastModifiedTime.Should().Be(expected);
    }

    [Fact]
    public void HavingJsonNodeRepresentingFullFile_WhenJFileIsParsed_FileHashIsReadCorrectly()
    {
        FileHash expected = new(new byte[] { 13, 6, 80 });
        jDummyObject.FileHash.Should().Be(expected);
    }
}