// DirectoryCompare
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

using System;
using System.Collections.Generic;
using System.Linq;
using DustInTheWind.DirectoryCompare.Domain;
using DustInTheWind.DirectoryCompare.Domain.Comparison;
using DustInTheWind.DirectoryCompare.Domain.DataAccess;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Domain.Logging;
using DustInTheWind.DirectoryCompare.Domain.Utils;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Application.Other.FindDuplicates
{
    public class FindDuplicatesRequestHandler : RequestHandler<FindDuplicatesRequest, DuplicatesAnalysis>
    {
        private readonly ISnapshotRepository snapshotRepository;
        private readonly IBlackListRepository blackListRepository;
        private readonly ILog log;

        public FindDuplicatesRequestHandler(ISnapshotRepository snapshotRepository, IBlackListRepository blackListRepository, ILog log)
        {
            this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
            this.blackListRepository = blackListRepository ?? throw new ArgumentNullException(nameof(blackListRepository));
            this.log = log ?? throw new ArgumentNullException(nameof(log));
        }

        protected override DuplicatesAnalysis Handle(FindDuplicatesRequest request)
        {
            log.WriteInfo("Searching for duplicates between pot '{0}' and '{1}'.", request.SnapshotLeft.PotName, request.SnapshotRight.PotName);

            FileDuplicates fileDuplicates = new FileDuplicates
            {
                FilesLeft = GetFiles(request.SnapshotLeft),
                FilesRight = string.IsNullOrEmpty(request.SnapshotRight.PotName) ? null : GetFiles(request.SnapshotRight),
                CheckFilesExist = request.CheckFilesExist
            };

            DuplicatesAnalysis duplicatesAnalysis = new DuplicatesAnalysis(fileDuplicates);

            duplicatesAnalysis.RunAsync();

            return duplicatesAnalysis;
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
}