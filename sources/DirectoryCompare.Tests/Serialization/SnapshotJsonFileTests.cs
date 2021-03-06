﻿// DirectoryCompare
// Copyright (C) 2017-2019 Dust in the Wind
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

using System;
using System.Globalization;
using System.IO;
using DustInTheWind.DirectoryCompare.Entities;
using DustInTheWind.DirectoryCompare.JsonHashesFile.JsonExport;
using DustInTheWind.DirectoryCompare.JsonHashesFile.Serialization;
using Newtonsoft.Json;
using NUnit.Framework;

namespace DustInTheWind.DirectoryCompare.Tests.Serialization
{
    [TestFixture]
    public class SnapshotJsonFileTests
    {
        [Test]
        public void SerializeEmptySnapshot()
        {
            SnapshotJsonFile snapshotJsonFile = new SnapshotJsonFile();
            snapshotJsonFile.Snapshot = new Snapshot
            {
                CreationTime = new DateTime(2019, 5, 8, 19, 17, 0, DateTimeKind.Utc),
                Name = "Snapshot 1",
                OriginalPath = @"c:\aaa"
            };

            const string expected = @"{
  ""serializer-id"": ""9e93055d-7bde-4f55-b340-dd5a4880d96e"",
  ""original-path"": ""c:\\aaa"",
  ""creation-time"": ""2019-05-08T19:17:00Z""
}";
            PerformTest(snapshotJsonFile, expected);
        }

        [Test]
        public void SerializeSnapshotWithOneDirectory()
        {
            SnapshotJsonFile snapshotJsonFile = new SnapshotJsonFile();
            snapshotJsonFile.Snapshot = new Snapshot
            {
                CreationTime = new DateTime(2019, 5, 8, 19, 17, 0, DateTimeKind.Utc),
                Name = "Snapshot 1",
                OriginalPath = @"c:\aaa"
            };
            snapshotJsonFile.Snapshot.Directories.Add(new HDirectory
            {
                Name = "directory-name"
            });

            const string expected = @"{
  ""serializer-id"": ""9e93055d-7bde-4f55-b340-dd5a4880d96e"",
  ""original-path"": ""c:\\aaa"",
  ""creation-time"": ""2019-05-08T19:17:00Z"",
  ""d"": [
    {
      ""n"": ""directory-name""
    }
  ]
}";
            PerformTest(snapshotJsonFile, expected);
        }

        [Test]
        public void SerializeSnapshotWithOneFile()
        {
            SnapshotJsonFile snapshotJsonFile = new SnapshotJsonFile();
            snapshotJsonFile.Snapshot = new Snapshot
            {
                CreationTime = new DateTime(2019, 5, 8, 19, 17, 0, DateTimeKind.Utc),
                Name = "Snapshot 1",
                OriginalPath = @"c:\aaa"
            };
            snapshotJsonFile.Snapshot.Files.Add(new HFile
            {
                Name = "file.extension",
                Hash = new byte[] { 0, 1, 2 }
            });

            const string expected = @"{
  ""serializer-id"": ""9e93055d-7bde-4f55-b340-dd5a4880d96e"",
  ""original-path"": ""c:\\aaa"",
  ""creation-time"": ""2019-05-08T19:17:00Z"",
  ""f"": [
    {
      ""n"": ""file.extension"",
      ""h"": ""AAEC""
    }
  ]
}";
            PerformTest(snapshotJsonFile, expected);
        }

        private static void PerformTest(SnapshotJsonFile snapshotJsonFile, string expected)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                StreamWriter streamWriter = new StreamWriter(memoryStream);
                snapshotJsonFile.Save(streamWriter);
                streamWriter.Flush();
                memoryStream.Flush();

                memoryStream.Position = 0;

                StreamReader streamReader = new StreamReader(memoryStream);
                string json = streamReader.ReadToEnd();

                Assert.That(json, Is.EqualTo(expected));
            }
        }
    }
}