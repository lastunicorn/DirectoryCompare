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
using DustInTheWind.DirectoryCompare.Ports.ImportExportAccess;
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
    private readonly IImportExport importExport;

    private int count;
    private DataSize totalSize;
    private readonly Stopwatch stopwatch = new();
    private IDuplicatesOutput duplicatesOutput;

    public FindDuplicatesUseCase(ISnapshotRepository snapshotRepository, IBlackListRepository blackListRepository,
        ILog log, IFileSystem fileSystem, IDuplicateFilesUi duplicateFilesUi, IImportExport importExport)
    {
        this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
        this.blackListRepository = blackListRepository ?? throw new ArgumentNullException(nameof(blackListRepository));
        this.log = log ?? throw new ArgumentNullException(nameof(log));
        this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        this.duplicateFilesUi = duplicateFilesUi ?? throw new ArgumentNullException(nameof(duplicateFilesUi));
        this.importExport = importExport ?? throw new ArgumentNullException(nameof(importExport));
    }

    public async Task Handle(FindDuplicatesRequest request, CancellationToken cancellationToken)
    {
        stopwatch.Restart();

        OpenOutputFile(request.OutputFileName);

        LogStart(request);
        await AnnounceStartToUser(request);
        ExportHeader(request);

        List<HFile> filesLeft = await GetFiles(request.SnapshotLeft);
        List<HFile> filesRight = await GetFiles(request.SnapshotRight);

        IEnumerable<FileGroup> duplicates = ComputeDuplicates(filesLeft, filesRight, request.CheckFilesExistence);

        foreach (FileGroup fileGroup in duplicates)
        {
            IEnumerable<FilePair> filePairs = fileGroup.EnumeratePairs();

            foreach (FilePair filePair in filePairs)
            {
                count++;
                totalSize += fileGroup.Size;
                
                await AnnounceDuplicateToUser(filePair);
            }

            ExportDuplicate(fileGroup);
        }

        stopwatch.Stop();

        await AnnounceFinishedToUser();
        CloseOutputFile();
    }

    private void OpenOutputFile(string outputFileName)
    {
        if (outputFileName != null)
            duplicatesOutput = importExport.OpenDuplicatesOutput(outputFileName);
    }

    private void LogStart(FindDuplicatesRequest request)
    {
        string potNameLeft = request.SnapshotLeft.PotName;
        string potNameRight = request.SnapshotRight.PotName;
        log.WriteInfo("Searching for duplicates between pot '{0}' and '{1}'.", potNameLeft, potNameRight);
    }

    private Task AnnounceStartToUser(FindDuplicatesRequest request)
    {
        DuplicateSearchStartedInfo info = new()
        {
            SnapshotLeft = request.SnapshotLeft,
            SnapshotRight = request.SnapshotRight
        };

        return duplicateFilesUi.AnnounceStart(info);
    }

    private void ExportHeader(FindDuplicatesRequest request)
    {
        if (duplicatesOutput == null)
            return;

        string potNameLeft = request.SnapshotLeft.PotName;
        string potNameRight = request.SnapshotRight.PotName;

        duplicatesOutput.WriteHeader(potNameLeft, potNameRight);
    }

    private async Task<List<HFile>> GetFiles(SnapshotLocation snapshotLocation)
    {
        SnapshotFiles snapshotFiles = new(snapshotLocation, snapshotRepository, blackListRepository);
        IEnumerable<HFile> hFiles = await snapshotFiles.Enumerate();
        return hFiles?.ToList();
    }

    private IEnumerable<FileGroup> ComputeDuplicates(List<HFile> filesLeft, List<HFile> filesRight, bool checkFilesExistence)
    {
        FileDuplicates fileDuplicates = new()
        {
            FilesLeft = filesLeft,
            FilesRight = filesRight
        };

        IEnumerable<FileGroup> duplicates = fileDuplicates.EnumerateGroups();

        if (checkFilesExistence)
        {
            duplicates = duplicates
                .Select(x =>
                {
                    IEnumerable<HFile> hFiles = x.Where(file => fileSystem.FileExists(file.GetOriginalPath()));
                    return new FileGroup(hFiles);
                })
                .Where(x => x.Count > 1);
        }

        return duplicates;
    }

    private Task AnnounceDuplicateToUser(FilePair filePair)
    {
        DuplicateFoundInfo duplicateFoundInfo = new()
        {
            FullPathLeft = filePair.FullPathLeft,
            FullPathRight = filePair.FullPathRight,
            Size = filePair.Size,
            Hash = filePair.Hash
        };

        return duplicateFilesUi.AnnounceDuplicate(duplicateFoundInfo);
    }

    private void ExportDuplicate(FileGroup fileGroup)
    {
        if (duplicatesOutput == null)
            return;

        Duplicate duplicate = new()
        {
            FullPaths = fileGroup
                .Select(x => x.GetOriginalPath())
                .ToList(),
            Size = fileGroup.Size,
            Hash = fileGroup.Hash
        };

        duplicatesOutput.WriteDuplicate(duplicate);
    }


    private Task AnnounceFinishedToUser()
    {
        DuplicateSearchFinishedInfo info = new()
        {
            DuplicateCount = count,
            TotalSize = totalSize,
            ElapsedTime = stopwatch.Elapsed
        };

        return duplicateFilesUi.AnnounceFinished(info);
    }

    private void CloseOutputFile()
    {
        duplicatesOutput?.Close();
    }
}