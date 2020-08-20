// DirectoryCompare
// Copyright (C) 2017-2019 Dust in the Wind
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

using DustInTheWind.DirectoryCompare.Domain.Comparison;
using DustInTheWind.DirectoryCompare.Domain.DataAccess;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using DustInTheWind.DirectoryCompare.Domain;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Domain.Utils;

namespace DustInTheWind.DirectoryCompare.Application.RemoveDuplicates
{
    public class RemoveDuplicatesRequestHandler : RequestHandler<RemoveDuplicatesRequest>
    {
        private readonly ISnapshotRepository snapshotRepository;
        private readonly IBlackListRepository blackListRepository;

        private RemoveDuplicatesRequest request;

        public RemoveDuplicatesRequestHandler(ISnapshotRepository snapshotRepository, IBlackListRepository blackListRepository)
        {
            this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
            this.blackListRepository = blackListRepository ?? throw new ArgumentNullException(nameof(blackListRepository));
        }

        protected override void Handle(RemoveDuplicatesRequest request)
        {
            this.request = request;
            
            PathCollection blackListPathsLeft = blackListRepository.Get(request.SnapshotLeft.PotName);
            BlackList blackListLeft = new BlackList(blackListPathsLeft);

            PathCollection blackListPathsRight = blackListRepository.Get(request.SnapshotRight.PotName);
            BlackList blackListRight = new BlackList(blackListPathsRight);

            FileDuplicates fileDuplicates = new FileDuplicates
            {
                FilesLeft = GetFiles(request.SnapshotLeft, blackListLeft),
                FilesRight = GetFiles(request.SnapshotRight, blackListRight),
                CheckFilesExist = true
            };

            RemoveDuplicates(fileDuplicates);
        }

        private List<HFile> GetFiles(SnapshotLocation snapshotLocation, BlackList blackList)
        {
            IEnumerable<HFile> files = snapshotRepository.EnumerateFiles(snapshotLocation, blackList);
            return files.ToList();
        }

        private void RemoveDuplicates(IEnumerable<FileDuplicate> fileDuplicates)
        {
            int fileRemovedCount = 0;
            DataSize totalSize = 0;

            foreach (FileDuplicate duplicate in fileDuplicates)
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
                        request.Exporter.WriteRemove(duplicate.FullPathLeft);
                        break;

                    case ComparisonSide.Right:
                        if (request.DestinationDirectory == null)
                            duplicate.DeleteRight();
                        else
                            duplicate.MoveRight(request.DestinationDirectory);
                        fileRemovedCount++;
                        totalSize += duplicate.Size;
                        request.Exporter.WriteRemove(duplicate.FullPathRight);
                        break;
                }
            }

            request.Exporter.WriteSummary(fileRemovedCount, totalSize);
        }
    }
}