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
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.JFiles.SnapshotFileModel;

namespace DustInTheWind.DirectoryCompare.DataAccess.Transformations
{
    internal static class JSnapshotExtensions
    {
        public static Snapshot ToSnapshot(this JSnapshot jSnapshot)
        {
            if (jSnapshot == null) throw new ArgumentNullException(nameof(jSnapshot));

            Snapshot snapshot = new()
            {
                Id = jSnapshot.AnalysisId, 
                OriginalPath = jSnapshot.OriginalPath,
                CreationTime = jSnapshot.CreationTime
            };

            IEnumerable<HDirectory> directories = jSnapshot.GetHDirectories();
            if (directories != null)
                snapshot.Directories.AddRange(directories);

            IEnumerable<HFile> files = jSnapshot.GetHFiles();
            if (files != null)
                snapshot.Files.AddRange(files);

            return snapshot;
        }

        private static IEnumerable<HDirectory> GetHDirectories(this JSnapshot jSnapshot)
        {
            return jSnapshot.Directories?
                .Select(x => x.ToHDirectory())
                .ToList();
        }

        private static IEnumerable<HFile> GetHFiles(this JSnapshot jSnapshot)
        {
            return jSnapshot.Files?
                .Select(x => x.ToHFile())
                .ToList();
        }
    }
}