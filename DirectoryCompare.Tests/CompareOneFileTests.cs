using NUnit.Framework;

namespace DirectoryCompare.Tests
{
    [TestFixture]
    public class CompareOneFileTests
    {
        #region OnlyInContainer1

        [Test]
        public void OnlyInContainer1_is_empty_if_both_containers_contain_one_identical_file()
        {
            Container container1 = new Container();
            container1.Files.Add(new XFile { Name = "File1", Hash = new byte[] { 0x01, 0x02, 0x03 } });

            Container container2 = new Container();
            container2.Files.Add(new XFile { Name = "File1", Hash = new byte[] { 0x01, 0x02, 0x03 } });

            ContainerComparer comparer = new ContainerComparer(container1, container2);
            comparer.Compare();

            Assert.That(comparer.OnlyInContainer1, Is.EqualTo(new string[0]));
        }

        [Test]
        public void OnlyInContainer1_contains_the_name_of_the_file_if_only_container1_has_one_file()
        {
            Container container1 = new Container();
            container1.Files.Add(new XFile { Name = "File1", Hash = new byte[] { 0x01, 0x02, 0x03 } });

            Container container2 = new Container();

            ContainerComparer comparer = new ContainerComparer(container1, container2);
            comparer.Compare();

            Assert.That(comparer.OnlyInContainer1, Is.EqualTo(new[] { "/File1" }));
        }

        [Test]
        public void OnlyInContainer1_is_empty_if_only_container2_has_one_file()
        {
            Container container1 = new Container();

            Container container2 = new Container();
            container2.Files.Add(new XFile { Name = "File1", Hash = new byte[] { 0x01, 0x02, 0x03 } });

            ContainerComparer comparer = new ContainerComparer(container1, container2);
            comparer.Compare();

            Assert.That(comparer.OnlyInContainer1, Is.EqualTo(new string[0]));
        }

        #endregion

        #region OnlyInContainer2

        [Test]
        public void OnlyInContainer2_is_empty_if_both_containers_contain_one_identical_file()
        {
            Container container1 = new Container();
            container1.Files.Add(new XFile { Name = "File1", Hash = new byte[] { 0x01, 0x02, 0x03 } });

            Container container2 = new Container();
            container2.Files.Add(new XFile { Name = "File1", Hash = new byte[] { 0x01, 0x02, 0x03 } });

            ContainerComparer comparer = new ContainerComparer(container1, container2);
            comparer.Compare();

            Assert.That(comparer.OnlyInContainer2, Is.EqualTo(new string[0]));
        }

        [Test]
        public void OnlyInContainer2_contains_the_name_of_the_file_if_only_container2_has_one_file()
        {
            Container container1 = new Container();

            Container container2 = new Container();
            container2.Files.Add(new XFile { Name = "File1", Hash = new byte[] { 0x01, 0x02, 0x03 } });

            ContainerComparer comparer = new ContainerComparer(container1, container2);
            comparer.Compare();

            Assert.That(comparer.OnlyInContainer2, Is.EqualTo(new[] { "/File1" }));
        }

        [Test]
        public void OnlyInContainer2_is_empty_if_only_container1_has_one_file()
        {
            Container container1 = new Container();
            container1.Files.Add(new XFile { Name = "File1", Hash = new byte[] { 0x01, 0x02, 0x03 } });

            Container container2 = new Container();

            ContainerComparer comparer = new ContainerComparer(container1, container2);
            comparer.Compare();

            Assert.That(comparer.OnlyInContainer2, Is.EqualTo(new string[0]));
        }

        #endregion
    }
}