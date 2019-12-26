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

using System;
using System.Collections.Generic;
using System.IO;
using DustInTheWind.DirectoryCompare.Domain.Comparison;
using DustInTheWind.DirectoryCompare.Domain.DataAccess;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Application.UseCases.RemoveDuplicates
{
    public class RemoveDuplicatesRequestHandler : RequestHandler<RemoveDuplicatesRequest>
    {
        private readonly IProjectRepository projectRepository;

        public RemoveDuplicatesRequestHandler(IProjectRepository projectRepository)
        {
            this.projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
        }

        protected override void Handle(RemoveDuplicatesRequest request)
        {
            Snapshot snapshotLeft = projectRepository.GetSnapshot(request.PathLeft);
            Snapshot snapshotRight = null;

            if (request.PathRight != null)
                snapshotRight = projectRepository.GetSnapshot(request.PathRight);

            FileDuplicates fileDuplicates = new FileDuplicates
            {
                SnapshotLeft = snapshotLeft,
                SnapshotRight = snapshotRight,
                CheckFilesExist = true
            };

            IEnumerable<FileDuplicate> duplicates = fileDuplicates.Compare();

            int removeCount = 0;
            long totalSize = 0;

            foreach (FileDuplicate duplicate in duplicates)
            {
                if (!duplicate.AreEqual)
                    continue;

                bool file1Exists = duplicate.File1Exists;
                bool file2Exists = duplicate.File2Exists;

                if (file1Exists && file2Exists)
                {
                    switch (request.FileToRemove)
                    {
                        case ComparisonSide.Left:
                            File.Delete(duplicate.FullPath1);
                            removeCount++;
                            totalSize += duplicate.Size;
                            request.Exporter.WriteRemove(duplicate.FullPath1);
                            break;

                        case ComparisonSide.Right:
                            File.Delete(duplicate.FullPath2);
                            removeCount++;
                            totalSize += duplicate.Size;
                            request.Exporter.WriteRemove(duplicate.FullPath2);
                            break;
                    }
                }
            }

            request.Exporter.WriteSummary(removeCount, totalSize);
        }
    }
}