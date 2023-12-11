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
        await AnnounceStart(request);

        List<HFile> filesLeft = await GetFiles(request.SnapshotLeft);
        List<HFile> filesRight = await GetFiles(request.SnapshotRight);

        IEnumerable<FilePair> duplicates = ComputeDuplicates(filesLeft, filesRight, request.CheckFilesExistence);
        await AnnounceDuplicates(duplicates);

        await AnnounceFinished();
    }

    private async Task AnnounceStart(FindDuplicatesRequest request)
    {
        string potNameLeft = request.SnapshotLeft.PotName;
        string potNameRight = request.SnapshotRight.PotName;
        log.WriteInfo("Searching for duplicates between pot '{0}' and '{1}'.", potNameLeft, potNameRight);

        await duplicateFilesUi.AnnounceStart(request.SnapshotLeft, request.SnapshotRight);
    }

    private async Task<List<HFile>> GetFiles(SnapshotLocation snapshotLocation)
    {
        SnapshotFiles snapshotFiles = new(snapshotLocation, snapshotRepository, blackListRepository);
        IEnumerable<HFile> hFiles = await snapshotFiles.Enumerate();
        return hFiles.ToList();
    }

    private IEnumerable<FilePair> ComputeDuplicates(List<HFile> filesLeft, List<HFile> filesRight, bool checkFilesExistence)
    {
        FileDuplicates fileDuplicates = new()
        {
            FilesLeft = filesLeft,
            FilesRight = filesRight
        };

        IEnumerable<FilePair> duplicates = fileDuplicates.Enumerate();

        if (checkFilesExistence)
        {
            duplicates = duplicates
                .Where(x => fileSystem.FileExists(x.FullPathLeft) && fileSystem.FileExists(x.FullPathRight));
        }

        return duplicates;
    }

    private async Task AnnounceDuplicates(IEnumerable<FilePair> duplicates)
    {
        foreach (FilePair filePair in duplicates)
        {
            count++;
            totalSize += filePair.Size;

            Ports.UserAccess.FilePairDto filePairDto = new()
            {
                FullPathLeft = filePair.FullPathLeft,
                FullPathRight = filePair.FullPathRight,
                Size = filePair.Size,
                Hash = filePair.Hash
            };

            await duplicateFilesUi.AnnounceDuplicate(filePairDto);
        }
    }

    private async Task AnnounceFinished()
    {
        await duplicateFilesUi.AnnounceFinished(count, totalSize);
    }
}