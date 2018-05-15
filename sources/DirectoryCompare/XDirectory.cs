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

using System;
using System.Collections.Generic;

namespace DustInTheWind.DirectoryCompare
{
    public class XDirectory : XItem
    {
        public List<XDirectory> Directories { get; set; }

        public List<XFile> Files { get; set; }

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