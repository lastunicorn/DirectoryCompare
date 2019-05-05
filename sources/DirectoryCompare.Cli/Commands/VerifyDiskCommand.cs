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

using DirectoryCompare.CliFramework;
using DustInTheWind.DirectoryCompare.Cli.ResultExporters;
using Newtonsoft.Json;
using System;
using System.IO;
using DustInTheWind.DirectoryCompare.InMemoryExport;

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
            ContainerDiskExport containerDiskExport = new ContainerDiskExport();
            DiskReader diskReader1 = new DiskReader(DiskPath, containerDiskExport);
            diskReader1.Starting += HandleDiskReaderStarting;
            diskReader1.Read();

            string json2 = File.ReadAllText(FilePath);
            XContainer xContainer2 = JsonConvert.DeserializeObject<XContainer>(json2);

            Compare(containerDiskExport.Container, xContainer2);
        }

        private void HandleDiskReaderStarting(object sender, DiskReaderStartingEventArgs e)
        {
            Console.WriteLine("Computed black list:");

            foreach (string blackListItem in e.BlackList)
                Console.WriteLine("- " + blackListItem);
        }

        private void Compare(XContainer container1, XContainer container2)
        {
            ContainerComparer comparer = new ContainerComparer(container1, container2);
            comparer.Compare();

            Exporter?.Export(comparer);
        }
    }
}