// DirectoryCompare
// Copyright (C) 2017-2024 Dust in the Wind
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

using DustInTheWind.DirectoryCompare.DataStructures;
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

    public async Task<PresentSnapshotResponse> Handle(PresentSnapshotRequest request, CancellationToken cancellationToken)
    {
        Snapshot snapshot = await snapshotRepository.Get(request.Location);

        if (snapshot == null)
            throw new SnapshotNotFoundException(request.Location);

        DataSize storageSize = await snapshotRepository.GetStorageSize(request.Location);
        HItemCounter itemCounter = snapshot.CountChildItems();

        DirectoryDto directoryToDisplay = GetDirectoryToDisplay(snapshot, request);

        return new PresentSnapshotResponse
        {
            PotName = request.Location.PotName,
            SnapshotId = snapshot.Id,
            OriginalPath = snapshot.OriginalPath,
            SnapshotCreationTime = snapshot.CreationTime,
            DirectoryPath = "/" + request.DirectoryPath,
            RootDirectory = directoryToDisplay,
            TotalFileCount = itemCounter.FileCount,
            TotalDirectoryCount = itemCounter.DirectoryCount,
            DataSize = itemCounter.DataSize,
            StorageSize = storageSize
        };
    }

    private static DirectoryDto GetDirectoryToDisplay(Snapshot snapshot, PresentSnapshotRequest request)
    {
        if (!request.DirectoryPath.IsEmpty)
        {
            HDirectory hDirectoryToReturn = snapshot.GetDirectory(request.DirectoryPath);
            int directoryLevel = request.DirectoryLevel > 0
                ? request.DirectoryLevel
                : -1;
            return new DirectoryDto(hDirectoryToReturn, directoryLevel);
        }

        if (request.DirectoryLevel > 0)
            return new DirectoryDto(snapshot, request.DirectoryLevel);

        return null;
    }
}