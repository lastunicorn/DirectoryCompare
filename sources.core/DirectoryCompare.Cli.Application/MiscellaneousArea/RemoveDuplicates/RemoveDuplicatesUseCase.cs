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

using DustInTheWind.DirectoryCompare.DataStructures;
using DustInTheWind.DirectoryCompare.Domain.Comparison;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Ports.DataAccess;
using DustInTheWind.DirectoryCompare.Ports.LogAccess;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Cli.Application.MiscellaneousArea.RemoveDuplicates;

public class RemoveDuplicatesUseCase : IRequestHandler<RemoveDuplicatesRequest>
{
    private readonly ISnapshotRepository snapshotRepository;
    private readonly IBlackListRepository blackListRepository;
    private readonly IRemoveDuplicatesLog removeDuplicatesLog;

    public RemoveDuplicatesUseCase(ISnapshotRepository snapshotRepository, IBlackListRepository blackListRepository, IRemoveDuplicatesLog removeDuplicatesLog)
    {
        this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
        this.blackListRepository = blackListRepository ?? throw new ArgumentNullException(nameof(blackListRepository));
        this.removeDuplicatesLog = removeDuplicatesLog ?? throw new ArgumentNullException(nameof(removeDuplicatesLog));
    }

    // The real work will run asynchronously, while the promise will raise events whenever something important
    // happened that the presentation layer should handle.
    public Task Handle(RemoveDuplicatesRequest request, CancellationToken cancellationToken)
    {
        DiskPathCollection blackListPathsLeft = blackListRepository.Get(request.SnapshotLeft.PotName);
        BlackList blackListLeft = new(blackListPathsLeft);

        DiskPathCollection blackListPathsRight = blackListRepository.Get(request.SnapshotRight.PotName);
        BlackList blackListRight = new(blackListPathsRight);

        FileDuplicates fileDuplicates = new()
        {
            FilesLeft = EnumerateFiles(request.SnapshotLeft, blackListLeft).ToList(),
            FilesRight = EnumerateFiles(request.SnapshotRight, blackListRight).ToList(),
            CheckFilesExistence = true
        };

        RemoveDuplicates(request, fileDuplicates);

        return Task.CompletedTask;
    }

    private IEnumerable<HFile> EnumerateFiles(SnapshotLocation snapshotLocation, BlackList blackList = null)
    {
        Snapshot snapshot = snapshotRepository.Get(snapshotLocation);

        return snapshot == null
            ? Enumerable.Empty<HFile>()
            : snapshot.EnumerateFiles(snapshotLocation.InternalPath, blackList);
    }

    private void RemoveDuplicates(RemoveDuplicatesRequest request, IEnumerable<FilePair> fileDuplicates)
    {
        int fileRemovedCount = 0;
        DataSize totalSize = 0;

        foreach (FilePair duplicate in fileDuplicates)
        {
            bool bothFilesExist = duplicate.FileLeftExists && duplicate.FileRightExists;

            if (!bothFilesExist)
            {
                // todo: announce that duplicate was not removed.
                continue;
            }

            switch (request.FileToRemove)
            {
                case ComparisonSide.Left:
                    if (request.DestinationDirectory == null)
                        duplicate.DeleteLeft();
                    else
                        duplicate.MoveLeft(request.DestinationDirectory);

                    fileRemovedCount++;
                    totalSize += duplicate.Size;
                    removeDuplicatesLog.WriteRemove(duplicate.FullPathLeft);
                    break;

                case ComparisonSide.Right:
                    if (request.DestinationDirectory == null)
                        duplicate.DeleteRight();
                    else
                        duplicate.MoveRight(request.DestinationDirectory);
                    fileRemovedCount++;
                    totalSize += duplicate.Size;
                    removeDuplicatesLog.WriteRemove(duplicate.FullPathRight);
                    break;
            }
        }

        removeDuplicatesLog.WriteSummary(fileRemovedCount, totalSize);
    }
}