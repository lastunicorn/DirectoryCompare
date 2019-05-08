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

using System.Collections.Generic;
using DustInTheWind.DirectoryCompare.Entities;
using NUnit.Framework;

namespace DustInTheWind.DirectoryCompare.Tests.ContainerComparerTests
{
    [TestFixture]
    public class IdenticalContainersWithSameFileTwiceTests
    {
        private HContainer container1;
        private HContainer container2;
        private ContainerComparer containerComparer;

        [SetUp]
        public void SetUp()
        {
            container1 = new HContainer();
            container1.Files.AddRange(new[]
            {
                new HFile { Name = "File1.txt", Hash = new byte[] { 1, 2, 3 } },
                new HFile { Name = "File2.txt", Hash = new byte[] { 1, 2, 3 } }
            });
            container2 = new HContainer();
            container2.Files.AddRange(new[]
            {
                new HFile { Name = "File1.txt", Hash = new byte[] { 1, 2, 3 } },
                new HFile { Name = "File2.txt", Hash = new byte[] { 1, 2, 3 } }
            });
            containerComparer = new ContainerComparer(container1, container2);
        }

        [Test]
        public void OnlyInContainer1_is_empty()
        {
            containerComparer.Compare();

            Assert.That(containerComparer.OnlyInContainer1, Is.Empty);
        }

        [Test]
        public void OnlyInContainer2_is_empty()
        {
            containerComparer.Compare();

            Assert.That(containerComparer.OnlyInContainer2, Is.Empty);
        }

        [Test]
        public void DifferentContent_is_empty()
        {
            containerComparer.Compare();

            Assert.That(containerComparer.DifferentContent, Is.Empty);
        }

        [Test]
        public void DifferentNames_contains_data()
        {
            containerComparer.Compare();

            Assert.That(containerComparer.DifferentNames, Is.EqualTo(new[]
            {
                new ItemComparison
                {
                    RootPath = "/",
                    Item1 = new HFile { Name = "File1.txt", Hash = new byte[] { 1, 2, 3 } },
                    Item2 = new HFile { Name = "File2.txt", Hash = new byte[] { 1, 2, 3 } }
                },
                new ItemComparison
                {
                    RootPath = "/",
                    Item1 = new HFile { Name = "File2.txt", Hash = new byte[] { 1, 2, 3 } },
                    Item2 = new HFile { Name = "File1.txt", Hash = new byte[] { 1, 2, 3 } }
                }
            }));
        }
    }
}