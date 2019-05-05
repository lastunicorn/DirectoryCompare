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

using System.Collections.Generic;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Application.Duplicates
{
    public class FindDuplicatesRequestHandler : RequestHandler<FindDuplicatesRequest>
    {
        protected override void Handle(FindDuplicatesRequest request)
        {
            DuplicatesProvider duplicatesProvider = new DuplicatesProvider
            {
                PathLeft = request.PathLeft,
                PathRight = request.PathRight,
                CheckFilesExist = request.CheckFilesExist
            };

            IEnumerable<Duplicate> duplicates = duplicatesProvider.Find();

            int duplicateCount = 0;
            long totalSize = 0;

            foreach (Duplicate duplicate in duplicates)
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