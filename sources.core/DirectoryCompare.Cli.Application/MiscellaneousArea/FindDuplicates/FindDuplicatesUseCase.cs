﻿// DirectoryCompare
// Copyright (C) 2017-2020 Dust in the Wind
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

using DustInTheWind.DirectoryCompare.Domain;
using DustInTheWind.DirectoryCompare.Domain.Comparison;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Domain.Utils;
using DustInTheWind.DirectoryCompare.Ports.DataAccess;
using DustInTheWind.DirectoryCompare.Ports.LogAccess;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Cli.Application.MiscellaneousArea.FindDuplicates;

public class FindDuplicatesUseCase : IRequestHandler<FindDuplicatesRequest, FileDuplicates>
{
    private readonly ISnapshotRepository snapshotRepository;
    private readonly IBlackListRepository blackListRepository;
    private readonly ILog log;

    public FindDuplicatesUseCase(ISnapshotRepository snapshotRepository, IBlackListRepository blackListRepository, ILog log)
    {
        this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
        this.blackListRepository = blackListRepository ?? throw new ArgumentNullException(nameof(blackListRepository));
        this.log = log ?? throw new ArgumentNullException(nameof(log));
    }

    public Task<FileDuplicates> Handle(FindDuplicatesRequest request, CancellationToken cancellationToken)
    {
        log.WriteInfo("Searching for duplicates between pot '{0}' and '{1}'.", request.SnapshotLeft.PotName, request.SnapshotRight.PotName);

        FileDuplicates response = new()
        {
            FilesLeft = GetFiles(request.SnapshotLeft),
            FilesRight = string.IsNullOrEmpty(request.SnapshotRight.PotName)
                ? null
                : GetFiles(request.SnapshotRight),
            CheckFilesExistence = request.CheckFilesExistence
        };

        return Task.FromResult(response);
    }

    private List<HFile> GetFiles(SnapshotLocation snapshotLocation)
    {
        BlackList blackList = GetBlackList(snapshotLocation);

        IEnumerable<HFile> files = snapshotRepository.EnumerateFiles(snapshotLocation, blackList);
        return files.ToList();
    }

    private BlackList GetBlackList(SnapshotLocation snapshotLocation)
    {
        if (snapshotLocation.PotName == null)
            return null;

        DiskPathCollection blackListPaths = blackListRepository.Get(snapshotLocation.PotName);
        return new BlackList(blackListPaths);
    }
}