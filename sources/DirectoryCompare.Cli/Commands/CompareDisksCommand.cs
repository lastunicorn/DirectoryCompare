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

using DustInTheWind.DirectoryCompare.Cli.ResultExporters;
using System;

namespace DustInTheWind.DirectoryCompare.Cli.Commands
{
    internal class CompareDisksCommand : ICommand
    {
        public ProjectLogger Logger { get; set; }
        public string Path1 { get; set; }
        public string Path2 { get; set; }
        public IComparisonExporter Exporter { get; set; }

        public string Name => "compare-disks";

        public void DisplayInfo()
        {
            Console.WriteLine("Compare paths:");
            Console.WriteLine(Path1);
            Console.WriteLine(Path2);
        }

        public void Initialize(Arguments arguments)
        {
            Logger = new ProjectLogger();
            Path1 = arguments[0];
            Path2 = arguments[1];
            Exporter = new ConsoleComparisonExporter();
        }

        public void Execute()
        {
            DiskReader diskReader1 = new DiskReader(Path1);
            diskReader1.Read();

            DiskReader diskReader2 = new DiskReader(Path2);
            diskReader2.Read();

            Compare(diskReader1.Container, diskReader2.Container);
        }

        private void Compare(XContainer xContainer1, XContainer xContainer2)
        {
            ContainerComparer comparer = new ContainerComparer(xContainer1, xContainer2);
            comparer.Compare();

            Exporter?.Export(comparer);
        }
    }
}