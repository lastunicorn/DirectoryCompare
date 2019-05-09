// DirectoryCompare
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
using DustInTheWind.DirectoryCompare.JsonHashesFile.JsonExport;
using Newtonsoft.Json;
using NUnit.Framework;

namespace DustInTheWind.DirectoryCompare.Tests.Serialization
{
    [TestFixture]
    public class JsonSnapshotReaderTests
    {
        [Test]
        public void Test()
        {
            string json = @"{
  ""serializer-id"": ""9e93055d-7bde-4f55-b340-dd5a4880d96e"",
  ""original-path"": ""c:\\aaa"",
  ""creation-time"": ""2019-05-08T19:17:00Z""
}";

            using (StringReader stringReader = new StringReader(json))
            using (JsonTextReader jsonTextReader = new JsonTextReader(stringReader))
            {
                SnapshotBuilder snapshotBuilder = new SnapshotBuilder();
                JsonSnapshotReader reader = new JsonSnapshotReader(jsonTextReader, snapshotBuilder);

                reader.Read();

                Assert.AreEqual("c:\\aaa", snapshotBuilder.Snapshot.OriginalPath);
                Assert.AreEqual(DateTime.Parse("2019-05-08T19:17:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal), snapshotBuilder.Snapshot.CreationTime);
            }
        }
    }
}