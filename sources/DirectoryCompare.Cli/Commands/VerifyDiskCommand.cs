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
using System.IO;
using DirectoryCompare.CliFramework;
using DustInTheWind.DirectoryCompare.Cli.ResultExporters;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.Cli.Commands
{
    internal class VerifyDiskCommand : ICommand
    {
        public ProjectLogger Logger { get; set; }
        public string DiskPath { get; set; }
        public string FilePath { get; set; }
        public IComparisonExporter Exporter { get; set; }

        public void DisplayInfo()
        {
            Console.WriteLine("Verify path: " + DiskPath);
        }

        public void Initialize(Arguments arguments)
        {
            Logger = new ProjectLogger();
            DiskPath = arguments[0];
            FilePath = arguments[1];
            Exporter = new ConsoleComparisonExporter();
        }

        public void Execute()
        {
            DiskReader diskReader1 = new DiskReader(DiskPath);
            diskReader1.Read();

            string json2 = File.ReadAllText(FilePath);
            XContainer xContainer2 = JsonConvert.DeserializeObject<XContainer>(json2);

            Compare(diskReader1.Container, xContainer2);
        }

        private void Compare(XContainer container1, XContainer container2)
        {
            ContainerComparer comparer = new ContainerComparer(container1, container2);
            comparer.Compare();

            Exporter?.Export(comparer);
        }
    }
}