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

using DustInTheWind.DirectoryCompare.Comparison;
using DustInTheWind.DirectoryCompare.JsonHashesFile.Serialization;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Application.Compare
{
    public class CompareSnapshotsRequestHandler : RequestHandler<CompareSnapshotsRequest>
    {
        protected override void Handle(CompareSnapshotsRequest request)
        {
            SnapshotJsonFile file1 = SnapshotJsonFile.Load(request.Path1);
            SnapshotJsonFile file2 = SnapshotJsonFile.Load(request.Path2);

            SnapshotComparer comparer = new SnapshotComparer(file1.Snapshot, file2.Snapshot);
            comparer.Compare();

            request.Exporter?.Export(comparer);
        }
    }
}