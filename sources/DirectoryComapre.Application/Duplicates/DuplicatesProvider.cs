// DirectoryCompare
// Copyright (C) 2017-2019 Dust in the Wind
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
using System.IO;
using DustInTheWind.DirectoryCompare.Entities;
using DustInTheWind.DirectoryCompare.JsonHashesFile.Serialization;

namespace DustInTheWind.DirectoryCompare.Application.Duplicates
{
    internal class DuplicatesProvider
    {
        public string PathLeft { get; set; }
        public string PathRight { get; set; }
        public bool CheckFilesExist { get; set; }

        public IEnumerable<Duplicate> Find()
        {
            JsonFileSerializer serializer = new JsonFileSerializer();
            HContainer hContainerLeft = serializer.ReadFromFile(PathLeft);

            List<Tuple<string, HFile>> filesLeft = new List<Tuple<string, HFile>>();
            Read(filesLeft, hContainerLeft, Path.DirectorySeparatorChar.ToString());

            if (PathRight == null)
            {
                return FindDuplicates(filesLeft, hContainerLeft);
            }
            else
            {
                HContainer hContainerRight = serializer.ReadFromFile(PathRight);

                List<Tuple<string, HFile>> filesRight = new List<Tuple<string, HFile>>();
                Read(filesRight, hContainerRight, Path.DirectorySeparatorChar.ToString());

                return FindDuplicates(filesLeft, filesRight, hContainerLeft, hContainerRight);
            }
        }

        private static void Read(ICollection<Tuple<string, HFile>> files, HDirectory hDirectory, string parentPath)
        {
            if (hDirectory.Files != null)
                foreach (HFile xFile in hDirectory.Files)
                {
                    string filePath = Path.Combine(parentPath, xFile.Name);
                    files.Add(new Tuple<string, HFile>(filePath, xFile));
                }

            if (hDirectory.Directories != null)
                foreach (HDirectory xSubDirectory in hDirectory.Directories)
                {
                    string subdirectoryPath = Path.Combine(parentPath, xSubDirectory.Name);
                    Read(files, xSubDirectory, subdirectoryPath);
                }
        }

        private IEnumerable<Duplicate> FindDuplicates(List<Tuple<string, HFile>> files, HContainer hContainer)
        {
            for (int i = 0; i < files.Count; i++)
            {
                for (int j = i + 1; j < files.Count; j++)
                {
                    Tuple<string, HFile> tupleLeft = files[i];
                    Tuple<string, HFile> tupleRight = files[j];

                    yield return new Duplicate(tupleLeft, tupleRight, CheckFilesExist, hContainer, hContainer);
                }
            }
        }

        private IEnumerable<Duplicate> FindDuplicates(List<Tuple<string, HFile>> filesLeft, List<Tuple<string, HFile>> filesRight, HContainer hContainerLeft, HContainer hContainerRight)
        {
            for (int i = 0; i < filesLeft.Count; i++)
            {
                for (int j = 0; j < filesRight.Count; j++)
                {
                    Tuple<string, HFile> tupleLeft = filesLeft[i];
                    Tuple<string, HFile> tupleRight = filesRight[j];

                    yield return new Duplicate(tupleLeft, tupleRight, CheckFilesExist, hContainerLeft, hContainerRight);
                }
            }
        }
    }
}