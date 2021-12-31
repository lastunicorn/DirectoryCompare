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

using System;
using System.IO;
using DustInTheWind.DirectoryCompare.DataAccess;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Domain.Utils;
using NUnit.Framework;

namespace DustInTheWind.DirectoryCompare.Tests.DataAccess
{
    [TestFixture]
    public class PotImportExportTests
    {
        [Test]
        public void SerializeEmptySnapshot()
        {
            Snapshot snapshot = new Snapshot
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
            PerformTest(snapshot, expected);
        }

        [Test]
        public void SerializeSnapshotWithOneDirectory()
        {
            Snapshot snapshot = new Snapshot
            {
                CreationTime = new DateTime(2019, 5, 8, 19, 17, 0, DateTimeKind.Utc),
                Name = "Snapshot 1",
                OriginalPath = @"c:\aaa"
            };
            snapshot.Directories.Add(new HDirectory
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
            PerformTest(snapshot, expected);
        }

        [Test]
        public void SerializeSnapshotWithOneFile()
        {
            Snapshot snapshot = new Snapshot
            {
                CreationTime = new DateTime(2019, 5, 8, 19, 17, 0, DateTimeKind.Utc),
                Name = "Snapshot 1",
                OriginalPath = @"c:\aaa"
            };
            snapshot.Files.Add(new HFile
            {
                Name = "file.extension",
                Size = DataSize.FromKilobytes(42),
                LastModifiedTime = new DateTime(2011, 05, 13, 12, 56, 20),
                Hash = new byte[] { 0, 1, 2 }
            });

            const string expected = @"{
  ""serializer-id"": ""9e93055d-7bde-4f55-b340-dd5a4880d96e"",
  ""original-path"": ""c:\\aaa"",
  ""creation-time"": ""2019-05-08T19:17:00Z"",
  ""f"": [
    {
      ""n"": ""file.extension"",
      ""s"": 43008,
      ""m"": ""2011-05-13T12:56:20"",
      ""h"": ""AAEC""
    }
  ]
}";
            PerformTest(snapshot, expected);
        }

        private static void PerformTest(Snapshot snapshot, string expected)
        {
            using MemoryStream memoryStream = new();
            using StreamWriter streamWriter = new(memoryStream);

            PotImportExport potImportExport = new();
            potImportExport.WriteSnapshot(snapshot, streamWriter);
            
            streamWriter.Flush();
            memoryStream.Flush();

            memoryStream.Position = 0;

            using StreamReader streamReader = new(memoryStream);
            string json = streamReader.ReadToEnd();

            Assert.That(json, Is.EqualTo(expected));
        }
    }
}