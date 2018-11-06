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

using DustInTheWind.DirectoryCompare.Serialization;
using System;
using System.Collections.Generic;
using System.IO;

namespace DustInTheWind.DirectoryCompare.Cli
{
    internal class DuplicatesProvider
    {
        public string PathLeft { get; set; }
        public string PathRight { get; set; }
        public bool CheckFilesExist { get; set; }

        public void DisplayInfo()
        {
        }

        public IEnumerable<Duplicate> Find()
        {
            JsonFileSerializer serializer = new JsonFileSerializer();
            XContainer xContainerLeft = serializer.ReadFromFile(PathLeft);

            List<Tuple<string, XFile>> filesLeft = new List<Tuple<string, XFile>>();
            Read(filesLeft, xContainerLeft, Path.DirectorySeparatorChar.ToString());

            if (PathRight == null)
            {
                return FindDuplicates(filesLeft, xContainerLeft);
            }
            else
            {
                XContainer xContainerRight = serializer.ReadFromFile(PathRight);

                List<Tuple<string, XFile>> filesRight = new List<Tuple<string, XFile>>();
                Read(filesRight, xContainerRight, Path.DirectorySeparatorChar.ToString());

                return FindDuplicates(filesLeft, filesRight, xContainerLeft, xContainerRight);
            }
        }

        private void Read(List<Tuple<string, XFile>> files, XDirectory xDirectory, string parentPath)
        {
            if (xDirectory.Files != null)
                foreach (XFile xFile in xDirectory.Files)
                {
                    string filePath = Path.Combine(parentPath, xFile.Name);
                    files.Add(new Tuple<string, XFile>(filePath, xFile));
                }

            if (xDirectory.Directories != null)
                foreach (XDirectory xSubDirectory in xDirectory.Directories)
                {
                    string subdirectoryPath = Path.Combine(parentPath, xSubDirectory.Name);
                    Read(files, xSubDirectory, subdirectoryPath);
                }
        }

        private IEnumerable<Duplicate> FindDuplicates(List<Tuple<string, XFile>> files, XContainer xContainer)
        {
            for (int i = 0; i < files.Count; i++)
            {
                for (int j = i + 1; j < files.Count; j++)
                {
                    Tuple<string, XFile> tupleLeft = files[i];
                    Tuple<string, XFile> tupleRight = files[j];

                    yield return new Duplicate(tupleLeft, tupleRight, CheckFilesExist, xContainer, xContainer);
                }
            }
        }

        private IEnumerable<Duplicate> FindDuplicates(List<Tuple<string, XFile>> filesLeft, List<Tuple<string, XFile>> filesRight, XContainer xContainerLeft, XContainer xContainerRight)
        {
            for (int i = 0; i < filesLeft.Count; i++)
            {
                for (int j = 0; j < filesRight.Count; j++)
                {
                    Tuple<string, XFile> tupleLeft = filesLeft[i];
                    Tuple<string, XFile> tupleRight = filesRight[j];

                    yield return new Duplicate(tupleLeft, tupleRight, CheckFilesExist, xContainerLeft, xContainerRight);
                }
            }
        }
    }
}