using System;

namespace DustInTheWind.DirectoryCompare.Domain.PotModel
{
    public class PotDoesNotExistException : Exception
    {
        private const string DefaultMessage = "There is no pot with the name '{0}'.";

        public PotDoesNotExistException(string potName)
            : base(string.Format(DefaultMessage, potName))
        {
        }
    }
}