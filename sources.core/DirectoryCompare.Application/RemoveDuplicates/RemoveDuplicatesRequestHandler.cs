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
using System.Linq;
using DustInTheWind.DirectoryCompare.Domain.Comparison;
using DustInTheWind.DirectoryCompare.Domain.DataAccess;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Application.RemoveDuplicates
{
    public class RemoveDuplicatesRequestHandler : RequestHandler<RemoveDuplicatesRequest>
    {
        private readonly ISnapshotRepository snapshotRepository;
        private readonly IPotRepository potRepository;

        public RemoveDuplicatesRequestHandler(ISnapshotRepository snapshotRepository, IPotRepository potRepository)
        {
            this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
            this.potRepository = potRepository ?? throw new ArgumentNullException(nameof(potRepository));
        }

        protected override void Handle(RemoveDuplicatesRequest request)
        {
            List<HFile> filesLeft = GetFiles(request.PathLeft);

            if (filesLeft == null)
                return;

            List<HFile> filesRight = GetFiles(request.PathRight);

            FileDuplicates fileDuplicates = new FileDuplicates
            {
                FilesLeft = filesLeft,
                FilesRight = filesRight,
                CheckFilesExist = true
            };

            int removeCount = 0;
            long totalSize = 0;

            foreach (FileDuplicate duplicate in fileDuplicates)
            {
                if (!duplicate.AreEqual)
                    continue;

                if (duplicate.File1Exists && duplicate.File2Exists)
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

        private List<HFile> GetFiles(string potPath)
        {
            if (potPath == null)
                return null;

            string potName = potPath;
            string filter = null;

            bool potExists = potRepository.Exists(potName);

            if (!potExists)
            {
                int pos = potPath.IndexOf('!');
                if (pos >= 0)
                {
                    potName = potPath.Substring(0, pos);
                    filter = potPath.Substring(pos + 1);

                    potExists = potRepository.Exists(potName);
                }

                if (!potExists)
                    return null;
            }

            Snapshot snapshot = snapshotRepository.GetLast(potName);

            return snapshot?.EnumerateFiles()
                .Where(x => filter == null || x.GetPath().StartsWith(filter))
                .ToList();
        }
    }
}