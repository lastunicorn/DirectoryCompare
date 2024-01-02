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
using DustInTheWind.DirectoryCompare.Ports.ImportExportAccess;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Cli.Application.MiscellaneousArea.PresentDuplicates;

internal class PresentDuplicatesUseCase : IRequestHandler<PresentDuplicatesRequest, PresentDuplicatesResponse>
{
    private readonly IImportExport importExport;

    public PresentDuplicatesUseCase(IImportExport importExport)
    {
        this.importExport = importExport ?? throw new ArgumentNullException(nameof(importExport));
    }

    public Task<PresentDuplicatesResponse> Handle(PresentDuplicatesRequest request, CancellationToken cancellationToken)
    {
        IDuplicatesInput duplicatesInput = importExport.OpenDuplicatesInput(request.FilePath);

        DuplicatesHeader duplicatesHeader = duplicatesInput.GetHeader();

        DataSizeComparer dataSizeComparer = new();
        SortedList<DataSize, FileGroup> fileGroups = new(dataSizeComparer);
        int totalDuplicatesCount = 0;
        DataSize totalSize = DataSize.Zero;

        foreach (FileDuplicateGroup fileDuplicateGroup in duplicatesInput.EnumerateDuplicates())
        {
            try
            {
                int fileCount = fileDuplicateGroup.FilePaths.Count;
                int duplicatesCount = ComputeDuplicatesCount(fileCount);

                totalDuplicatesCount += duplicatesCount;
                totalSize += duplicatesCount * fileDuplicateGroup.FileSize;

                FileGroup fileGroup = new()
                {
                    FilePaths = fileDuplicateGroup.FilePaths,
                    FileSize = fileDuplicateGroup.FileSize,
                    FileHash = fileDuplicateGroup.FileHash
                };

                fileGroups.Add(fileGroup.FileSize, fileGroup);
            }
            catch
            {
            }
        }

        PresentDuplicatesResponse response = new()
        {
            PotnameLeft = duplicatesHeader.PotNameLeft,
            PotnameRight = duplicatesHeader.PotNameRight,
            Duplicates = fileGroups.Values,
            DuplicateCount = totalDuplicatesCount,
            TotalSize = totalSize
        };

        return Task.FromResult(response);
    }

    private static int ComputeDuplicatesCount(int fileCount)
    {
        int value = 0;

        for (int i = 1; i < fileCount; i++)
            value += i;

        return value;
    }
}