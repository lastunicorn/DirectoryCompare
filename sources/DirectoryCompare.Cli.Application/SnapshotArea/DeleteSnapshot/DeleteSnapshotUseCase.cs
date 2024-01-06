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

using DustInTheWind.DirectoryCompare.Ports.DataAccess;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Cli.Application.SnapshotArea.DeleteSnapshot;

public class DeleteSnapshotUseCase : IRequestHandler<DeleteSnapshotRequest>
{
    private readonly ISnapshotRepository snapshotRepository;

    public DeleteSnapshotUseCase(ISnapshotRepository snapshotRepository)
    {
        this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
    }

    public async Task Handle(DeleteSnapshotRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Location.PotName))
            throw new Exception("Pot name was not provided.");

        if (request.Location.SnapshotIndex.HasValue)
        {
            await snapshotRepository.DeleteByIndex(request.Location.PotName, request.Location.SnapshotIndex.Value);
        }
        else if (request.Location.SnapshotDate.HasValue)
        {
            DateTime searchedDate = request.Location.SnapshotDate.Value;

            bool foundEndDeleted = await snapshotRepository.DeleteByExactDateTime(request.Location.PotName, searchedDate);

            if (foundEndDeleted)
                return;

            if (searchedDate.TimeOfDay == TimeSpan.Zero)
                await snapshotRepository.DeleteSingleByDate(request.Location.PotName, searchedDate);
        }
        else
        {
            await snapshotRepository.DeleteLast(request.Location.PotName);
        }
    }
}