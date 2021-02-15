using System;
using System.IO;
using DustInTheWind.DirectoryCompare.Domain;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Domain.Utils;
using DustInTheWind.DirectoryCompare.JFiles;
using DustInTheWind.DirectoryCompare.JFiles.SnapshotFileModel;
using Newtonsoft.Json;
using NUnit.Framework;

namespace DustInTheWind.DirectoryCompare.Tests.Serialization
{
    [TestFixture]
    public class JFileReaderTests
    {
        private const string Json = @"{
  ""n"": ""filename.extension"",
  ""s"": ""15"",
  ""m"": ""2011-12-21T11:14:16Z"",
  ""h"": ""DQZQ""
}";

        [Test]
        public void HavingJsonNodeRepresentingFullFile_WhenJFileIsParsed_FileNameIsReadCorrectly()
        {
            JFileFields fields = ReadAllFieldsFrom(Json);

            Assert.That(fields.FileName, Is.EqualTo("filename.extension"));
        }

        [Test]
        public void HavingJsonNodeRepresentingFullFile_WhenJFileIsParsed_FileSizeIsReadCorrectly()
        {
            JFileFields fields = ReadAllFieldsFrom(Json);

            Assert.That(fields.FileSize, Is.EqualTo(DataSize.FromBytes(15)));
        }

        [Test]
        public void HavingJsonNodeRepresentingFullFile_WhenJFileIsParsed_LastModifiedTimeIsReadCorrectly()
        {
            JFileFields fields = ReadAllFieldsFrom(Json);

            Assert.That(fields.LastModifiedTime, Is.EqualTo(new DateTime(2011, 12, 21, 11, 14, 16)));
        }

        [Test]
        public void HavingJsonNodeRepresentingFullFile_WhenJFileIsParsed_FileHashIsReadCorrectly()
        {
            JFileFields fields = ReadAllFieldsFrom(Json);

            Assert.That(fields.FileHash, Is.EqualTo(new FileHash(new byte[] { 13, 6, 80 })));
        }

        private static JFileFields ReadAllFieldsFrom(string json)
        {
            using StringReader stringReader = new StringReader(json);
            using JsonTextReader jsonTextReader = new JsonTextReader(stringReader);
            jsonTextReader.Read();

            JFileFields fields = new JFileFields();

            JFileReader jFileReader = new JFileReader(jsonTextReader);

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
}