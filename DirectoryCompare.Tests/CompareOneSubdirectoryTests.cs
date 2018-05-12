using NUnit.Framework;

namespace DirectoryCompare.Tests
{
    [TestFixture]
    public class CompareOneSubdirectoryTests
    {
        #region OnlyInContainer1

        [Test]
        public void OnlyInContainer1_is_empty_if_both_containers_contain_one_identical_subdir()
        {
            Container container1 = new Container();
            XDirectory xDirectory1 = new XDirectory("Dir1");
            container1.Directories.Add(xDirectory1);

            Container container2 = new Container();
            XDirectory xDirectory2 = new XDirectory("Dir1");
            container2.Directories.Add(xDirectory2);

            ContainerComparer comparer = new ContainerComparer(container1, container2);
            comparer.Compare();

            Assert.That(comparer.OnlyInContainer1, Is.EqualTo(new string[0]));
        }

        [Test]
        public void OnlyInContainer1_contains_the_name_of_the_subdir_if_only_container1_has_one_subdir()
        {
            Container container1 = new Container();
            XDirectory xDirectory1 = new XDirectory("Dir1");
            container1.Directories.Add(xDirectory1);

            Container container2 = new Container();

            ContainerComparer comparer = new ContainerComparer(container1, container2);
            comparer.Compare();

            Assert.That(comparer.OnlyInContainer1, Is.EqualTo(new[] { "/Dir1/" }));
        }

        [Test]
        public void OnlyInContainer1_is_empty_if_only_container2_has_one_subdir()
        {
            Container container1 = new Container();

            Container container2 = new Container();
            XDirectory xDirectory2 = new XDirectory("Dir1");
            container2.Directories.Add(xDirectory2);

            ContainerComparer comparer = new ContainerComparer(container1, container2);
            comparer.Compare();

            Assert.That(comparer.OnlyInContainer1, Is.EqualTo(new string[0]));
        }

        #endregion

        #region OnlyInContainer2

        [Test]
        public void OnlyInContainer2_is_empty_if_both_containers_contain_one_identical_subdir()
        {
            Container container1 = new Container();
            XDirectory xDirectory1 = new XDirectory("Dir1");
            container1.Directories.Add(xDirectory1);

            Container container2 = new Container();
            XDirectory xDirectory2 = new XDirectory("Dir1");
            container2.Directories.Add(xDirectory2);

            ContainerComparer comparer = new ContainerComparer(container1, container2);
            comparer.Compare();

            Assert.That(comparer.OnlyInContainer2, Is.EqualTo(new string[0]));
        }

        [Test]
        public void OnlyInContainer2_contains_the_name_of_the_subdir_if_only_container2_has_one_subdir()
        {
            Container container1 = new Container();

            Container container2 = new Container();
            XDirectory xDirectory2 = new XDirectory("Dir1");
            container2.Directories.Add(xDirectory2);

            ContainerComparer comparer = new ContainerComparer(container1, container2);
            comparer.Compare();

            Assert.That(comparer.OnlyInContainer2, Is.EqualTo(new[] { "/Dir1/" }));
        }

        [Test]
        public void OnlyInContainer2_is_empty_if_only_container1_has_one_subdir()
        {
            Container container1 = new Container();
            XDirectory xDirectory1 = new XDirectory("Dir1");
            container1.Directories.Add(xDirectory1);

            Container container2 = new Container();

            ContainerComparer comparer = new ContainerComparer(container1, container2);
            comparer.Compare();

            Assert.That(comparer.OnlyInContainer2, Is.EqualTo(new string[0]));
        }

        #endregion
    }
}