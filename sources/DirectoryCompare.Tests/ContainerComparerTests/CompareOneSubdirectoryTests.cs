// DirectoryCompare
// Copyright (C) 2017 Dust in the Wind
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

using NUnit.Framework;
using System.Collections.Generic;

namespace DustInTheWind.DirectoryCompare.Tests.ContainerComparerTests
{
    [TestFixture]
    public class CompareOneSubdirectoryTests
    {
        #region OnlyInContainer1

        [Test]
        public void OnlyInContainer1_is_empty_if_both_containers_contain_one_identical_subdir()
        {
            XContainer container1 = new XContainer();
            XDirectory xDirectory1 = new XDirectory("Dir1");
            container1.Directories = new List<XDirectory> { xDirectory1 };

            XContainer container2 = new XContainer();
            XDirectory xDirectory2 = new XDirectory("Dir1");
            container2.Directories = new List<XDirectory> { xDirectory2 };

            ContainerComparer comparer = new ContainerComparer(container1, container2);
            comparer.Compare();

            Assert.That(comparer.OnlyInContainer1, Is.EqualTo(new string[0]));
        }

        [Test]
        public void OnlyInContainer1_contains_the_name_of_the_subdir_if_only_container1_has_one_subdir()
        {
            XContainer container1 = new XContainer();
            XDirectory xDirectory1 = new XDirectory("Dir1");
            container1.Directories = new List<XDirectory> { xDirectory1 };

            XContainer container2 = new XContainer();

            ContainerComparer comparer = new ContainerComparer(container1, container2);
            comparer.Compare();

            Assert.That(comparer.OnlyInContainer1, Is.EqualTo(new[] { "/Dir1/" }));
        }

        [Test]
        public void OnlyInContainer1_is_empty_if_only_container2_has_one_subdir()
        {
            XContainer container1 = new XContainer();

            XContainer container2 = new XContainer();
            XDirectory xDirectory2 = new XDirectory("Dir1");
            container2.Directories = new List<XDirectory> { xDirectory2 };

            ContainerComparer comparer = new ContainerComparer(container1, container2);
            comparer.Compare();

            Assert.That(comparer.OnlyInContainer1, Is.EqualTo(new string[0]));
        }

        #endregion

        #region OnlyInContainer2

        [Test]
        public void OnlyInContainer2_is_empty_if_both_containers_contain_one_identical_subdir()
        {
            XContainer container1 = new XContainer();
            XDirectory xDirectory1 = new XDirectory("Dir1");
            container1.Directories = new List<XDirectory> { xDirectory1 };

            XContainer container2 = new XContainer();
            XDirectory xDirectory2 = new XDirectory("Dir1");
            container2.Directories = new List<XDirectory> { xDirectory2 };

            ContainerComparer comparer = new ContainerComparer(container1, container2);
            comparer.Compare();

            Assert.That(comparer.OnlyInContainer2, Is.EqualTo(new string[0]));
        }

        [Test]
        public void OnlyInContainer2_contains_the_name_of_the_subdir_if_only_container2_has_one_subdir()
        {
            XContainer container1 = new XContainer();

            XContainer container2 = new XContainer();
            XDirectory xDirectory2 = new XDirectory("Dir1");
            container2.Directories = new List<XDirectory> { xDirectory2 };

            ContainerComparer comparer = new ContainerComparer(container1, container2);
            comparer.Compare();

            Assert.That(comparer.OnlyInContainer2, Is.EqualTo(new[] { "/Dir1/" }));
        }

        [Test]
        public void OnlyInContainer2_is_empty_if_only_container1_has_one_subdir()
        {
            XContainer container1 = new XContainer();
            XDirectory xDirectory1 = new XDirectory("Dir1");
            container1.Directories = new List<XDirectory> { xDirectory1 };

            XContainer container2 = new XContainer();

            ContainerComparer comparer = new ContainerComparer(container1, container2);
            comparer.Compare();

            Assert.That(comparer.OnlyInContainer2, Is.EqualTo(new string[0]));
        }

        #endregion
    }
}