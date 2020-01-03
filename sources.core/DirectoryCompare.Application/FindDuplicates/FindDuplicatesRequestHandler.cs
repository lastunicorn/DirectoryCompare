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
using DustInTheWind.DirectoryCompare.Domain.Comparison;
using DustInTheWind.DirectoryCompare.Domain.DataAccess;
using DustInTheWind.DirectoryCompare.Domain.SomeInterfaces;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Application.FindDuplicates
{
    public class FindDuplicatesRequestHandler : RequestHandler<FindDuplicatesRequest>
    {
        private readonly ISnapshotRepository snapshotRepository;

        public FindDuplicatesRequestHandler(ISnapshotRepository snapshotRepository)
        {
            this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
        }

        protected override void Handle(FindDuplicatesRequest request)
        {
            FileDuplicates fileDuplicates = new FileDuplicates(snapshotRepository)
            {
                SnapshotLeft = request.SnapshotLeft,
                SnapshotRight = request.SnapshotRight,
                CheckFilesExist = request.CheckFilesExist
            };

            ExportDuplicates(fileDuplicates, request.Exporter);
        }

        private static void ExportDuplicates(IEnumerable<FileDuplicate> fileDuplicates, IDuplicatesExporter exporter)
        {
            int duplicateCount = 0;
            long totalSize = 0;

            foreach (FileDuplicate duplicate in fileDuplicates)
            {
                duplicateCount++;
                totalSize += duplicate.Size;
                exporter.WriteDuplicate(duplicate);
            }

            exporter.WriteSummary(duplicateCount, totalSize);
        }
    }
}