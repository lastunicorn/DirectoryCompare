// Directory Compare
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

namespace DustInTheWind.DirectoryCompare.Cli.Application.MiscellaneousArea.FindChange;

internal class FindChangeUseCase : IRequestHandler<FindChangeRequest, FindChangeResponse>
{
    private readonly ISnapshotRepository snapshotRepository;

    public FindChangeUseCase(ISnapshotRepository snapshotRepository)
    {
        this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
    }

    public async Task<FindChangeResponse> Handle(FindChangeRequest request, CancellationToken cancellationToken)
    {
        IAsyncEnumerable<Snapshot> snapshots = snapshotRepository.GetByPot(request.SnapshotLocation.PotName);

        SnapshotPath path = request.SnapshotLocation.InternalPath;

        List<HFileState> changeDates = path.IsEmpty
            ? new List<HFileState>()
            : await FindChanges(snapshots, path)
                .Reverse()
                .ToListAsync(cancellationToken: cancellationToken);

        return new FindChangeResponse
        {
            Path = path,
            Changes = changeDates
        };
    }

    private static async IAsyncEnumerable<HFileState> FindChanges(IAsyncEnumerable<Snapshot> snapshots, SnapshotPath filePath)
    {
        HFile previousFile = null;

        await foreach (Snapshot snapshot in snapshots.Reverse())
        {
            HFile file = snapshot.GetFile(filePath);

            if (file == null)
            {
                yield return new HFileState
                {
                    SnapshotDateTime = snapshot.CreationTime,
                    FileExists = false,
                    FileIsChanged = false
                };
            }
            else
            {
                if (previousFile == null)
                {
                    yield return new HFileState
                    {
                        SnapshotDateTime = snapshot.CreationTime,
                        FileExists = true,
                        FileIsChanged = false
                    };
                }
                else
                {
                    if (previousFile.Hash == file.Hash)
                    {
                        yield return new HFileState
                        {
                            SnapshotDateTime = snapshot.CreationTime,
                            FileExists = true,
                            FileIsChanged = false
                        };
                    }
                    else
                    {
                        yield return new HFileState
                        {
                            SnapshotDateTime = snapshot.CreationTime,
                            FileExists = true,
                            FileIsChanged = true
                        };
                    }
                }
            }

            previousFile = file;
        }
    }
}