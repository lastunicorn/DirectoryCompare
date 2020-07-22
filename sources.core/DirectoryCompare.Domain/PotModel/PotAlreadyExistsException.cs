using System;

namespace DustInTheWind.DirectoryCompare.Domain.PotModel
{
    public class PotAlreadyExistsException : Exception
    {
        private const string DefaultMessage = "Another pot with the same name already exists.";

        public PotAlreadyExistsException()
            : base(DefaultMessage)
        {
        }
    }
}