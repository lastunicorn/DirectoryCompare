﻿// DirectoryCompare
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

using System.IO;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare
{
    internal class VerifyDiskCommand : ICommand
    {
        public string DiskPath { get; set; }
        public string FilePath { get; set; }

        public void Execute()
        {
            DiskReader diskReader1 = new DiskReader(DiskPath);
            diskReader1.Read();

            string json2 = File.ReadAllText(FilePath);
            Container container2 = JsonConvert.DeserializeObject<Container>(json2);

            Compare(diskReader1.Container, container2);
        }

        private static void Compare(Container container1, Container container2)
        {
            ContainerComparer comparer = new ContainerComparer(container1, container2);
            comparer.Compare();

            Program.DisplayResults(comparer);
        }
    }
}