using System;
using System.Runtime.Serialization;

namespace DustInTheWind.DirectoryCompare.Domain
{
    [Serializable]
    public class DirectoryCompareException : Exception
    {
        public DirectoryCompareException()
        {
        }

        public DirectoryCompareException(string message)
            : base(message)
        {
        }

        public DirectoryCompareException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected DirectoryCompareException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}