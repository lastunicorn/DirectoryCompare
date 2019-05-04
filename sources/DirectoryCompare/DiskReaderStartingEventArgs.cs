using System;

namespace DustInTheWind.DirectoryCompare
{
    public class DiskReaderStartingEventArgs : EventArgs
    {
        public PathCollection BlackList { get; }

        public DiskReaderStartingEventArgs(PathCollection blackList)
        {
            BlackList = blackList;
        }
    }
}