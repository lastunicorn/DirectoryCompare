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
using DustInTheWind.DirectoryCompare.Comparison;
using DustInTheWind.DirectoryCompare.DataAccess;
using DustInTheWind.DirectoryCompare.Entities;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Application.UseCases.FindDuplicates
{
    public class FindDuplicatesRequestHandler : RequestHandler<FindDuplicatesRequest>
    {
        private readonly IProjectRepository projectRepository;

        public FindDuplicatesRequestHandler(IProjectRepository projectRepository)
        {
            this.projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
        }

        protected override void Handle(FindDuplicatesRequest request)
        {
            Snapshot snapshotLeft = projectRepository.GetSnapshot(request.PathLeft);
            Snapshot snapshotRight = null;

            if (request.PathRight != null)
                snapshotRight = projectRepository.GetSnapshot(request.PathRight);

            FileDuplicates fileDuplicates = new FileDuplicates
            {
                SnapshotLeft = snapshotLeft,
                SnapshotRight = snapshotRight,
                CheckFilesExist = request.CheckFilesExist
            };

            IEnumerable<FileDuplicate> duplicates = fileDuplicates.Compare();

            int duplicateCount = 0;
            long totalSize = 0;

            foreach (FileDuplicate duplicate in duplicates)
            {
                if (duplicate.AreEqual)
                {
                    duplicateCount++;
                    totalSize += duplicate.Size;
                    request.Exporter.WriteDuplicate(duplicate.FullPath1, duplicate.FullPath2, duplicate.Size);
                }
            }

            request.Exporter.WriteSummary(duplicateCount, totalSize);
        }
    }
}