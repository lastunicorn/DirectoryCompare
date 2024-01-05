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

using DustInTheWind.DirectoryCompare.Cli.Application.MiscellaneousArea.FindDuplicates;
using DustInTheWind.DirectoryCompare.DataStructures;
using DustInTheWind.DirectoryCompare.Domain.Comparison;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Ports.DataAccess;
using DustInTheWind.DirectoryCompare.Ports.FileSystemAccess;
using DustInTheWind.DirectoryCompare.Ports.LogAccess;
using DustInTheWind.DirectoryCompare.Ports.UserAccess;
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

    public async Task Handle(RemoveDuplicatesRequest request, CancellationToken cancellationToken)
    {
        List<HFile> filesLeft = await GetFiles(request.SnapshotLeft);
        List<HFile> filesRight = await GetFiles(request.SnapshotRight);

        IEnumerable<FilePair> duplicates = ComputeDuplicates(filesLeft, filesRight);
        RemoveDuplicates(request, duplicates);
    }

    private async Task<List<HFile>> GetFiles(SnapshotLocation snapshotLocation)
    {
        SnapshotFiles snapshotFiles = new(snapshotLocation, snapshotRepository, blackListRepository);
        IEnumerable<HFile> hFiles = await snapshotFiles.Enumerate();
        return hFiles.ToList();
    }

    private static IEnumerable<FilePair> ComputeDuplicates(List<HFile> filesLeft, List<HFile> filesRight)
    {
        FileDuplicates fileDuplicates = new()
        {
            FilesLeft = filesLeft,
            FilesRight = filesRight
        };

        return fileDuplicates.EnumeratePairs();
    }

    private void RemoveDuplicates(RemoveDuplicatesRequest request, IEnumerable<FilePair> fileDuplicates)
    {
        AnnounceStart(request);

        FileRemoveReport report = new();

        foreach (FilePair duplicate in fileDuplicates)
        {
            AnnounceDuplicateFound(duplicate);

            bool leftFileExists = fileSystem.FileExists(duplicate.FullPathLeft);
            bool rightFileExists = fileSystem.FileExists(duplicate.FullPathRight);

            if (!leftFileExists && !rightFileExists)
            {
                removeDuplicatesLog.WriteActionNoFileExists();
                continue;
            }

            switch (request.FileToRemove)
            {
                case ComparisonSide.Left:
                    if (!rightFileExists)
                    {
                        removeDuplicatesLog.WriteActionFileToKeepDoesNotExist("right");
                    }
                    else if (!leftFileExists)
                    {
                        removeDuplicatesLog.WriteActionFileIsAlreadyRemoved("left");
                    }
                    else if (request.PurgatoryDirectory == null)
                    {
                        duplicate.DeleteLeft();
                        removeDuplicatesLog.WriteActionFileDeleted("left");
                    }
                    else
                    {
                        duplicate.MoveLeft(request.PurgatoryDirectory);
                        removeDuplicatesLog.WriteActionFileMoved("left");
                    }

                    report.FileRemovedCount++;
                    report.TotalSize += duplicate.Size;
                    break;

                case ComparisonSide.Right:
                    if (!leftFileExists)
                    {
                        removeDuplicatesLog.WriteActionFileToKeepDoesNotExist("left");
                    }
                    else if (!rightFileExists)
                    {
                        removeDuplicatesLog.WriteActionFileIsAlreadyRemoved("right");
                    }
                    else if (request.PurgatoryDirectory == null)
                    {
                        duplicate.DeleteRight();
                        removeDuplicatesLog.WriteActionFileDeleted("right");
                    }
                    else
                    {
                        duplicate.MoveRight(request.PurgatoryDirectory);
                        removeDuplicatesLog.WriteActionFileMoved("right");
                    }

                    report.FileRemovedCount++;
                    report.TotalSize += duplicate.Size;
                    break;
            }
        }

        AnnounceFinish(report);
    }

    private void AnnounceStart(RemoveDuplicatesRequest request)
    {
        RemoveDuplicatesPlan removeDuplicatesPlan = new()
        {
            SnapshotLeft = request.SnapshotLeft.ToString(),
            SnapshotRight = request.SnapshotRight.ToString(),
            RemovePart = request.FileToRemove.ToString(),
            PurgatoryDirectory = request.PurgatoryDirectory
        };

        removeDuplicatesLog.WritePlanInfo(removeDuplicatesPlan);
    }

    private void AnnounceDuplicateFound(FilePair duplicate)
    {
        string pathLeft = duplicate.FullPathLeft;
        string pathRight = duplicate.FullPathRight;

        removeDuplicatesLog.DuplicateFound(pathLeft, pathRight);
    }

    private void AnnounceFinish(FileRemoveReport report)
    {
        int fileRemovedCount = report.FileRemovedCount;
        DataSize totalSize = report.TotalSize;

        removeDuplicatesLog.WriteSummary(fileRemovedCount, totalSize);
    }
}