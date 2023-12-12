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

using System.Diagnostics;
using DustInTheWind.DirectoryCompare.DataStructures;
using DustInTheWind.DirectoryCompare.Domain.Comparison;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Ports.DataAccess;
using DustInTheWind.DirectoryCompare.Ports.FileSystemAccess;
using DustInTheWind.DirectoryCompare.Ports.LogAccess;
using DustInTheWind.DirectoryCompare.Ports.UserAccess;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Cli.Application.MiscellaneousArea.FindDuplicates;

public class FindDuplicatesUseCase : IRequestHandler<FindDuplicatesRequest>
{
    private readonly ISnapshotRepository snapshotRepository;
    private readonly IBlackListRepository blackListRepository;
    private readonly ILog log;
    private readonly IFileSystem fileSystem;
    private readonly IDuplicateFilesUi duplicateFilesUi;

    private int count;
    private DataSize totalSize;
    private readonly Stopwatch stopwatch = new();

    public FindDuplicatesUseCase(ISnapshotRepository snapshotRepository, IBlackListRepository blackListRepository,
        ILog log, IFileSystem fileSystem, IDuplicateFilesUi duplicateFilesUi)
    {
        this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
        this.blackListRepository = blackListRepository ?? throw new ArgumentNullException(nameof(blackListRepository));
        this.log = log ?? throw new ArgumentNullException(nameof(log));
        this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        this.duplicateFilesUi = duplicateFilesUi ?? throw new ArgumentNullException(nameof(duplicateFilesUi));
    }

    public async Task Handle(FindDuplicatesRequest request, CancellationToken cancellationToken)
    {
        stopwatch.Restart();
        await AnnounceStart(request);

        List<HFile> filesLeft = await GetFiles(request.SnapshotLeft);
        List<HFile> filesRight = await GetFiles(request.SnapshotRight);

        IEnumerable<FilePair> duplicates = ComputeDuplicates(filesLeft, filesRight, request.CheckFilesExistence);
        await AddToResponse(duplicates);

        stopwatch.Stop();
        await AnnounceFinished();
    }

    private Task AnnounceStart(FindDuplicatesRequest request)
    {
        string potNameLeft = request.SnapshotLeft.PotName;
        string potNameRight = request.SnapshotRight.PotName;
        log.WriteInfo("Searching for duplicates between pot '{0}' and '{1}'.", potNameLeft, potNameRight);

        DuplicateSearchStartedInfo info = new()
        {
            SnapshotLeft = request.SnapshotLeft,
            SnapshotRight = request.SnapshotRight
        };

        return duplicateFilesUi.AnnounceStart(info);
    }

    private async Task<List<HFile>> GetFiles(SnapshotLocation snapshotLocation)
    {
        if (string.IsNullOrEmpty(snapshotLocation.PotName))
            return null;

        BlackList blackList = await GetBlackList(snapshotLocation.PotName);
        Snapshot snapshot = await snapshotRepository.Get(snapshotLocation);

        IEnumerable<HFile> files = snapshot == null
            ? Enumerable.Empty<HFile>()
            : snapshot.EnumerateFiles(snapshotLocation.InternalPath, blackList);

        return files.ToList();
    }

    private async Task<BlackList> GetBlackList(string potName)
    {
        if (potName == null)
            return null;

        IEnumerable<IBlackItem> blackListPaths = (await blackListRepository.Get(potName))
            .Select(x => new PathBlackItem(x));

        IEnumerable<IBlackItem> blackListHashes = (await blackListRepository.GetDuplicateExcludes(potName))
            .Select(x => new FileHashBlackItem(x));

        IEnumerable<IBlackItem> blackListItems = blackListPaths.Concat(blackListHashes);
        return new BlackList(blackListItems);
    }

    private IEnumerable<FilePair> ComputeDuplicates(List<HFile> filesLeft, List<HFile> filesRight, bool checkFilesExistence)
    {
        IEnumerable<FilePair> duplicates = new FileDuplicates
        {
            FilesLeft = filesLeft,
            FilesRight = filesRight
        };

        if (checkFilesExistence)
        {
            duplicates = duplicates
                .Where(x => fileSystem.FileExists(x.FullPathLeft) && fileSystem.FileExists(x.FullPathRight));
        }

        return duplicates;
    }

    private async Task AddToResponse(IEnumerable<FilePair> duplicates)
    {
        foreach (FilePair filePair in duplicates)
        {
            count++;
            totalSize += filePair.Size;

            DuplicateFoundInfo duplicateFoundInfo = new()
            {
                FullPathLeft = filePair.FullPathLeft,
                FullPathRight = filePair.FullPathRight,
                Size = filePair.Size,
                Hash = filePair.Hash
            };

            await duplicateFilesUi.AnnounceDuplicate(duplicateFoundInfo);
        }
    }

    private Task AnnounceFinished()
    {
        DuplicateSearchFinishedInfo info = new()
        {
            DuplicateCount = count,
            TotalSize = totalSize,
            ElapsedTime = stopwatch.Elapsed
        };

        return duplicateFilesUi.AnnounceFinished(info);
    }
}