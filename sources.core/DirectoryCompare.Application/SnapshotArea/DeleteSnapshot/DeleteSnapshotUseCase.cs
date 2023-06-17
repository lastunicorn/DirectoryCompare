// DirectoryCompare
// Copyright (C) 2017-2020 Dust in the Wind
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
using DustInTheWind.DirectoryCompare.Ports.DataAccess;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Application.SnapshotArea.DeleteSnapshot
{
    public class DeleteSnapshotUseCase : RequestHandler<DeleteSnapshotRequest>
    {
        private readonly ISnapshotRepository snapshotRepository;

        public DeleteSnapshotUseCase(ISnapshotRepository snapshotRepository)
        {
            this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
        }

        protected override void Handle(DeleteSnapshotRequest request)
        {
            if (string.IsNullOrEmpty(request.Location.PotName))
                throw new Exception("Pot name was not provided.");

            if (request.Location.SnapshotIndex.HasValue)
            {
                snapshotRepository.DeleteByIndex(request.Location.PotName, request.Location.SnapshotIndex.Value);
            }
            else if (request.Location.SnapshotDate.HasValue)
            {
                DateTime searchedDate = request.Location.SnapshotDate.Value;

                bool foundEndDeleted = snapshotRepository.DeleteByExactDateTime(request.Location.PotName, searchedDate);

                if (foundEndDeleted)
                    return;

                if (searchedDate.TimeOfDay == TimeSpan.Zero)
                    snapshotRepository.DeleteSingleByDate(request.Location.PotName, searchedDate);
            }
            else
            {
                snapshotRepository.DeleteLast(request.Location.PotName);
            }
        }
    }
}