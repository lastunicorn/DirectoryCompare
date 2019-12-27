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
using DustInTheWind.DirectoryCompare.Domain.Comparison;
using DustInTheWind.DirectoryCompare.Domain.DataAccess;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Application.UseCases.CompareSnapshots
{
    public class CompareSnapshotsRequestHandler : RequestHandler<CompareSnapshotsRequest, SnapshotComparer>
    {
        private readonly IProjectRepository projectRepository;
        private readonly ISnapshotRepository snapshotRepository;

        public CompareSnapshotsRequestHandler(IProjectRepository projectRepository, ISnapshotRepository snapshotRepository)
        {
            this.projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
            this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
        }

        protected override SnapshotComparer Handle(CompareSnapshotsRequest request)
        {
            Snapshot snapshot1 = snapshotRepository.GetLast(request.PotName1);
            Snapshot snapshot2 = snapshotRepository.GetLast(request.PotName2);

            SnapshotComparer comparer = new SnapshotComparer(snapshot1, snapshot2);
            comparer.Compare();

            return comparer;
        }
    }
}