﻿// DirectoryCompare
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
using System.Linq;
using DustInTheWind.DirectoryCompare.Domain.Comparison;
using DustInTheWind.DirectoryCompare.Domain.DataAccess;
using DustInTheWind.DirectoryCompare.Domain.Entities;
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
            List<HFile> filesLeft = GetFiles(request.Left);

            if (filesLeft == null)
                return;

            List<HFile> filesRight = GetFiles(request.Right);

            IEnumerable<FileDuplicate> fileDuplicates = filesRight == null
                ? GetDuplicates(filesLeft, request.CheckFilesExist)
                : GetDuplicates(filesLeft, filesRight, request.CheckFilesExist);

            ExportDuplicates(fileDuplicates, request.Exporter);
        }

        private List<HFile> GetFiles(string potName)
        {
            if (potName == null)
                return null;

            Snapshot snapshotRight = snapshotRepository.GetLast(potName);
            return snapshotRight?.EnumerateFiles().ToList();
        }

        private static IEnumerable<FileDuplicate> GetDuplicates(IReadOnlyList<HFile> files, bool checkFilesExist)
        {
            for (int i = 0; i < files.Count; i++)
            {
                HFile fileLeft = files[i];

                for (int j = i + 1; j < files.Count; j++)
                {
                    HFile fileRight = files[j];

                    FileDuplicate fileDuplicate = new FileDuplicate(fileLeft, fileRight, checkFilesExist);

                    if (fileDuplicate.AreEqual)
                        yield return fileDuplicate;
                }
            }
        }

        private static IEnumerable<FileDuplicate> GetDuplicates(IReadOnlyCollection<HFile> filesLeft, IReadOnlyCollection<HFile> filesRight, bool checkFilesExist)
        {
            foreach (HFile fileLeft in filesLeft)
            foreach (HFile fileRight in filesRight)
            {
                FileDuplicate fileDuplicate = new FileDuplicate(fileLeft, fileRight, checkFilesExist);

                if (fileDuplicate.AreEqual)
                    yield return fileDuplicate;
            }
        }

        private static void ExportDuplicates(IEnumerable<FileDuplicate> fileDuplicates, IDuplicatesExporter exporter)
        {
            int duplicateCount = 0;
            long totalSize = 0;

            foreach (FileDuplicate duplicate in fileDuplicates)
            {
                duplicateCount++;
                totalSize += duplicate.Size;
                exporter.WriteDuplicate(duplicate.FullPath1, duplicate.FullPath2, duplicate.Size);
            }

            exporter.WriteSummary(duplicateCount, totalSize);
        }
    }
}