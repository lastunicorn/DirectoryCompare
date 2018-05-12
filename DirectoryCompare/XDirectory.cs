using System;
using System.Collections.Generic;

namespace DirectoryCompare
{
    public class XDirectory : XItem
    {
        public List<XDirectory> Directories { get; set; } = new List<XDirectory>();

        public List<XFile> Files { get; set; } = new List<XFile>();

        public XDirectory()
        {
        }

        public XDirectory(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        //public IEnumerator<XItem> GetEnumerator()
        //{
        //    if (Files != null)
        //        foreach (XFile xFile in Files)
        //            yield return xFile;

        //    if (Directories != null)
        //        foreach (XDirectory xDirectory in Directories)
        //            yield return xDirectory;
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return GetEnumerator();
        //}
    }
}