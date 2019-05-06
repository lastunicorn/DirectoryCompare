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
using DustInTheWind.DirectoryCompare.DiskAnalysis;
using DustInTheWind.DirectoryCompare.InMemoryExport;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Application.Compare
{
    public class CompareDisksRequestHandler : RequestHandler<CompareDisksRequest>
    {
        protected override void Handle(CompareDisksRequest request)
        {
            ContainerDiskExport containerDiskExport1 = new ContainerDiskExport();
            DiskReader diskReader1 = new DiskReader(request.Path1, containerDiskExport1);
            diskReader1.Starting += HandleDiskReaderStarting;
            diskReader1.Read();

            ContainerDiskExport containerDiskExport2 = new ContainerDiskExport();
            DiskReader diskReader2 = new DiskReader(request.Path2, containerDiskExport2);
            diskReader2.Starting += HandleDiskReaderStarting;
            diskReader2.Read();

            ContainerComparer comparer = new ContainerComparer(containerDiskExport1.Container, containerDiskExport2.Container);
            comparer.Compare();

            request.Exporter.Export(comparer);
        }

        private static void HandleDiskReaderStarting(object sender, DiskReaderStartingEventArgs e)
        {
            Console.WriteLine("Computed black list:");

            foreach (string blackListItem in e.BlackList)
                Console.WriteLine("- " + blackListItem);
        }
    }
}