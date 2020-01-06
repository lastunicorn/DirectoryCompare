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

namespace DustInTheWind.DirectoryCompare.Application.RemoveDuplicates
{
    public class RemoveDuplicatesRequestHandler : RequestHandler<RemoveDuplicatesRequest>
    {
        private readonly ISnapshotRepository snapshotRepository;

        private RemoveDuplicatesRequest request;

        public RemoveDuplicatesRequestHandler(ISnapshotRepository snapshotRepository)
        {
            this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
        }

        protected override void Handle(RemoveDuplicatesRequest request)
        {
            this.request = request;

            FileDuplicates fileDuplicates = new FileDuplicates(snapshotRepository)
            {
                SnapshotLeft = request.SnapshotLeft,
                SnapshotRight = request.SnapshotRight,
                CheckFilesExist = true
            };

            RemoveDuplicates(fileDuplicates);
        }

        private void RemoveDuplicates(IEnumerable<FileDuplicate> fileDuplicates)
        {
            int fileRemovedCount = 0;
            long totalSize = 0;

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