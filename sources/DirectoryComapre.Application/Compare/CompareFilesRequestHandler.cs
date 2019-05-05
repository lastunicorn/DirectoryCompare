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

using DustInTheWind.DirectoryCompare.Entities;
using DustInTheWind.DirectoryCompare.JsonHashesFile.Serialization;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Application.Compare
{
    public class CompareFilesRequestHandler : RequestHandler<CompareFilesRequest>
    {
        protected override void Handle(CompareFilesRequest request)
        {
            JsonFileSerializer serializer = new JsonFileSerializer();

            // todo: must find a way to dynamically detect the serialization type.

            HContainer hContainer1 = serializer.ReadFromFile(request.Path1);
            HContainer hContainer2 = serializer.ReadFromFile(request.Path2);

            //string json1 = File.ReadAllText(Path1);
            //HContainer hContainer1 = JsonConvert.DeserializeObject<HContainer>(json1);

            //string json2 = File.ReadAllText(Path2);
            //Container container2 = JsonConvert.DeserializeObject<Container>(json2);

            ContainerComparer comparer = new ContainerComparer(hContainer1, hContainer2);
            comparer.Compare();

            request.Exporter?.Export(comparer);
        }
    }
}