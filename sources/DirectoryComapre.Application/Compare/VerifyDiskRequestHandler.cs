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
using DustInTheWind.DirectoryCompare.Entities;
using DustInTheWind.DirectoryCompare.InMemoryExport;
using DustInTheWind.DirectoryCompare.JsonHashesFile.Serialization;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Application.Compare
{
    public class VerifyDiskRequestHandler : RequestHandler<VerifyDiskRequest>
    {
        protected override void Handle(VerifyDiskRequest request)
        {
            ContainerDiskExport containerDiskExport = new ContainerDiskExport();
            DiskReader diskReader1 = new DiskReader(request.DiskPath, containerDiskExport);
            diskReader1.Starting += HandleDiskReaderStarting;
            diskReader1.Read();

            JsonFileSerializer serializer = new JsonFileSerializer();
            HContainer hContainer2 = serializer.ReadFromFile(request.FilePath);

            ContainerComparer comparer = new ContainerComparer(containerDiskExport.Container, hContainer2);
            comparer.Compare();

            request.Exporter?.Export(comparer);
        }

        private static void HandleDiskReaderStarting(object sender, DiskReaderStartingEventArgs e)
        {
            Console.WriteLine("Computed black list:");

            foreach (string blackListItem in e.BlackList)
                Console.WriteLine("- " + blackListItem);
        }
    }
}