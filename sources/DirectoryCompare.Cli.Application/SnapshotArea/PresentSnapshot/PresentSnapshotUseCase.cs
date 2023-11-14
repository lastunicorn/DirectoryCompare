// DirectoryCompare
// Copyright (C) 2017-2023 Dust in the Wind
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

using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Ports.DataAccess;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Cli.Application.SnapshotArea.PresentSnapshot;

public class PresentSnapshotUseCase : IRequestHandler<PresentSnapshotRequest, PresentSnapshotResponse>
{
    private readonly ISnapshotRepository snapshotRepository;

    public PresentSnapshotUseCase(ISnapshotRepository snapshotRepository)
    {
        this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
    }

    public Task<PresentSnapshotResponse> Handle(PresentSnapshotRequest request, CancellationToken cancellationToken)
    {
        Snapshot snapshot = snapshotRepository.Get(request.Location);

        if (snapshot == null)
            throw new Exception("Invalid snapshot part. Verify that the pot name and snapshot identifier were provided correctly.");

        PresentSnapshotResponse response = new()
        {
            PotName = request.Location.PotName,
            SnapshotId = snapshot.Id,
            OriginalPath = snapshot.OriginalPath,
            SnapshotCreationTime = snapshot.CreationTime,
            RootDirectory = new DirectoryDto(snapshot)
        };

        return Task.FromResult(response);
    }
}