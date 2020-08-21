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
using System.Collections.Generic;
using System.Linq;
using DustInTheWind.DirectoryCompare.Domain.DataAccess;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Application.GetSnapshot
{
    public class GetSnapshotRequestHandler : RequestHandler<GetSnapshotRequest, Snapshot>
    {
        private readonly ISnapshotRepository snapshotRepository;

        public GetSnapshotRequestHandler(ISnapshotRepository snapshotRepository)
        {
            this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
        }

        protected override Snapshot Handle(GetSnapshotRequest request)
        {
            if (string.IsNullOrEmpty(request.Location.PotName))
                throw new Exception("Pot name was not provided.");

            if (request.Location.SnapshotIndex.HasValue)
                return snapshotRepository.GetByIndex(request.Location.PotName, request.Location.SnapshotIndex.Value);

            if (request.Location.SnapshotDate.HasValue)
            {
                DateTime searchedDate = request.Location.SnapshotDate.Value;

                Snapshot snapshot = snapshotRepository.GetByExactDateTime(request.Location.PotName, searchedDate);

                if (snapshot == null && searchedDate.TimeOfDay == TimeSpan.Zero)
                {
                    List<Snapshot> snapshots = snapshotRepository.GetByDate(request.Location.PotName, searchedDate)
                        .ToList();

                    if (snapshots.Count == 1)
                        snapshot = snapshots[0];
                    else if (snapshots.Count > 1)
                        throw new Exception($"There are multiple snapshots that match the specified date. Pot = {request.Location.PotName}; Date = {searchedDate}");
                }

                return snapshot;
            }

            return snapshotRepository.GetLast(request.Location.PotName);
        }
    }
}