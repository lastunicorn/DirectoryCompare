using DustInTheWind.DirectoryCompare.Domain.Entities;

namespace DustInTheWind.DirectoryCompare.Tests.Domain.Entities
{
    internal static class HItemExtensions
    {
        public static HItem CreateFile(string name)
        {
            return new HFile
            {
                Name = name
            };
        }

        public static HItem CreateDirectory(string name)
        {
            return new HDirectory
            {
                Name = name
            };
        }

        public static HItem PlaceInto(this HItem hItem, string parentName)
        {
            HDirectory parentDirectory = new HDirectory
            {
                Name = parentName
            };

            hItem.Parent = parentDirectory;

            return parentDirectory;
        }
    }
}