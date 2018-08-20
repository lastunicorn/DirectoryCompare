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
using DustInTheWind.DirectoryCompare.JsonSerialization;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.Cli.Commands
{
    internal class CompareFilesCommand : ICommand
    {
        public ProjectLogger Logger { get; set; }
        public string Path1 { get; set; }
        public string Path2 { get; set; }
        public IComparisonExporter Exporter { get; set; }

        public void DisplayInfo()
        {
        }

        public void Execute()
        {
            JsonFileSerializer serializer = new JsonFileSerializer();

            //Container container1 = serializer.ReadFromFile(Path1);
            Container container2 = serializer.ReadFromFile(Path2);

            string json1 = File.ReadAllText(Path1);
            Container container1 = JsonConvert.DeserializeObject<Container>(json1);

            //string json2 = File.ReadAllText(Path2);
            //Container container2 = JsonConvert.DeserializeObject<Container>(json2);

            Compare(container1, container2);
        }

        private void Compare(Container container1, Container container2)
        {
            ContainerComparer comparer = new ContainerComparer(container1, container2);
            comparer.Compare();

            Exporter?.Export(comparer);
        }
    }
}