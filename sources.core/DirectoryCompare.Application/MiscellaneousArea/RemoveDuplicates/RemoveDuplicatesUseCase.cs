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
using DustInTheWind.DirectoryCompare.Domain.Comparison;
using DustInTheWind.DirectoryCompare.Domain.DataAccess;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Domain.Utils;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Application.MiscellaneousArea.RemoveDuplicates
{
    public class RemoveDuplicatesUseCase : RequestHandler<RemoveDuplicatesRequest>
    {
        private readonly ISnapshotRepository snapshotRepository;
        private readonly IBlackListRepository blackListRepository;

        private RemoveDuplicatesRequest request;

        public RemoveDuplicatesUseCase(ISnapshotRepository snapshotRepository, IBlackListRepository blackListRepository)
        {
            this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
            this.blackListRepository = blackListRepository ?? throw new ArgumentNullException(nameof(blackListRepository));
        }

        // todo: Instead of receiving an IRemoveDuplicatesExporter, make this handler async and return a promise.
        // The real work will run asynchronously, while the promise will raise events whenever something important
        // happened that the presentation layer should handle.
        protected override void Handle(RemoveDuplicatesRequest request)
        {
            this.request = request;
            
            DiskPathCollection blackListPathsLeft = blackListRepository.Get(request.SnapshotLeft.PotName);
            BlackList blackListLeft = new(blackListPathsLeft);

            DiskPathCollection blackListPathsRight = blackListRepository.Get(request.SnapshotRight.PotName);
            BlackList blackListRight = new(blackListPathsRight);

            FileDuplicates fileDuplicates = new()
            {
                FilesLeft = snapshotRepository.EnumerateFiles(request.SnapshotLeft, blackListLeft).ToList(),
                FilesRight = snapshotRepository.EnumerateFiles(request.SnapshotRight, blackListRight).ToList(),
                CheckFilesExistance = true
            };

            RemoveDuplicates(fileDuplicates);
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