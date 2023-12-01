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
using DustInTheWind.DirectoryCompare.Ports.FileSystemAccess;
using DustInTheWind.DirectoryCompare.Ports.LogAccess;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Cli.Application.MiscellaneousArea.RemoveDuplicates;

public class RemoveDuplicatesUseCase : IRequestHandler<RemoveDuplicatesRequest>
{
    private readonly ISnapshotRepository snapshotRepository;
    private readonly IBlackListRepository blackListRepository;
    private readonly IRemoveDuplicatesLog removeDuplicatesLog;
    private readonly IFileSystem fileSystem;

    public RemoveDuplicatesUseCase(ISnapshotRepository snapshotRepository, IBlackListRepository blackListRepository,
        IRemoveDuplicatesLog removeDuplicatesLog, IFileSystem fileSystem)
    {
        this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
        this.blackListRepository = blackListRepository ?? throw new ArgumentNullException(nameof(blackListRepository));
        this.removeDuplicatesLog = removeDuplicatesLog ?? throw new ArgumentNullException(nameof(removeDuplicatesLog));
        this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }

    // This is a draft. Not yet finished.
    public async Task Handle(RemoveDuplicatesRequest request, CancellationToken cancellationToken)
    {
        IEnumerable<PathBlackItem> blackListPathsLeft = (await blackListRepository.Get(request.SnapshotLeft.PotName))
            .Select(x => new PathBlackItem(x));
        BlackList blackListLeft = new(blackListPathsLeft);

        IEnumerable<PathBlackItem> blackListPathsRight = (await blackListRepository.Get(request.SnapshotRight.PotName))
            .Select(x => new PathBlackItem(x));
        BlackList blackListRight = new(blackListPathsRight);

        FileDuplicates fileDuplicates = new()
        {
            FilesLeft = (await EnumerateFiles(request.SnapshotLeft, blackListLeft))
                .ToList(),
            FilesRight = (await EnumerateFiles(request.SnapshotRight, blackListRight))
                .ToList()
        };

        RemoveDuplicates(request, fileDuplicates);
    }

    private async Task<IEnumerable<HFile>> EnumerateFiles(SnapshotLocation snapshotLocation, BlackList blackList = null)
    {
        Snapshot snapshot = await snapshotRepository.Get(snapshotLocation);

        return snapshot == null
            ? Enumerable.Empty<HFile>()
            : snapshot.EnumerateFiles(snapshotLocation.InternalPath, blackList);
    }

    private void RemoveDuplicates(RemoveDuplicatesRequest request, IEnumerable<FilePair> fileDuplicates)
    {
        RemoveDuplicatesPlan removeDuplicatesPlan = new()
        {
            SnapshotLeft = request.SnapshotLeft.ToString(),
            SnapshotRight = request.SnapshotRight.ToString(),
            RemovePart = request.FileToRemove.ToString(),
            PurgatoryDirectory = request.PurgatoryDirectory
        };

        removeDuplicatesLog.WritePlanInfo(removeDuplicatesPlan);

        int fileRemovedCount = 0;
        DataSize totalSize = 0;

        foreach (FilePair duplicate in fileDuplicates)
        {
            removeDuplicatesLog.DuplicateFound(duplicate.FullPathLeft, duplicate.FullPathRight);

            bool leftFileExists = fileSystem.FileExists(duplicate.FullPathLeft);
            bool rightFileExists = fileSystem.FileExists(duplicate.FullPathRight);

            if (!leftFileExists && !rightFileExists)
            {
                removeDuplicatesLog.WriteActionNoFileExists();
                continue;
            }

            bool fileToKeepDoesNotExist = (!leftFileExists && request.FileToRemove == ComparisonSide.Right) ||
                                          (!rightFileExists && request.FileToRemove == ComparisonSide.Left);
            if (fileToKeepDoesNotExist)
            {
                removeDuplicatesLog.WriteActionFileToKeepDoesNotExist();
                continue;
            }

            bool fileIsAlreadyRemoved = (!leftFileExists && request.FileToRemove == ComparisonSide.Left) ||
                                        (!rightFileExists && request.FileToRemove == ComparisonSide.Right);
            if (fileIsAlreadyRemoved)
            {
                removeDuplicatesLog.WriteActionFileIsAlreadyRemoved();
                continue;
            }

            switch (request.FileToRemove)
            {
                case ComparisonSide.Left:
                    if (request.PurgatoryDirectory == null)
                    {
                        duplicate.DeleteLeft();
                        removeDuplicatesLog.WriteActionFileDeleted("left");
                    }
                    else
                    {
                        duplicate.MoveLeft(request.PurgatoryDirectory);
                        removeDuplicatesLog.WriteActionFileMoved("left");
                    }

                    fileRemovedCount++;
                    totalSize += duplicate.Size;
                    break;

                case ComparisonSide.Right:
                    if (request.PurgatoryDirectory == null)
                    {
                        duplicate.DeleteRight();
                        removeDuplicatesLog.WriteActionFileDeleted("right");
                    }
                    else
                    {
                        duplicate.MoveRight(request.PurgatoryDirectory);
                        removeDuplicatesLog.WriteActionFileMoved("right");
                    }

                    fileRemovedCount++;
                    totalSize += duplicate.Size;
                    break;
            }
        }

        removeDuplicatesLog.WriteSummary(fileRemovedCount, totalSize);
    }
}