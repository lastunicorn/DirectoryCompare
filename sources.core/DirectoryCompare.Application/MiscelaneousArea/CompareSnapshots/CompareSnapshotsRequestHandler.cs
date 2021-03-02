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
using DustInTheWind.DirectoryCompare.Domain.Comparison;
using DustInTheWind.DirectoryCompare.Domain.DataAccess;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.RequestR;

namespace DustInTheWind.DirectoryCompare.Application.MiscelaneousArea.CompareSnapshots
{
    public class CompareSnapshotsRequestHandler : IRequestHandler<CompareSnapshotsRequest, CompareSnapshotsResponse>
    {
        private readonly ISnapshotRepository snapshotRepository;

        public CompareSnapshotsRequestHandler(ISnapshotRepository snapshotRepository)
        {
            this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
        }

        public CompareSnapshotsResponse Handle(CompareSnapshotsRequest request)
        {
            Snapshot snapshot1 = RetrieveSnapshot(request.PotName1);
            Snapshot snapshot2 = RetrieveSnapshot(request.PotName2);
            SnapshotComparer comparer = CompareSnapshots(snapshot1, snapshot2);
            string exportDirectoryPath = ExportToDiskIfRequested(comparer, request);

            return new CompareSnapshotsResponse
            {
                SnapshotComparer = comparer,
                ExportDirectoryPath = exportDirectoryPath
            };
        }

        private Snapshot RetrieveSnapshot(string potName)
        {
            Snapshot snapshot = snapshotRepository.GetLast(potName);

            if (snapshot == null)
                throw new Exception($"There is no pot with the name '{potName}'.");

            return snapshot;
        }

        private static SnapshotComparer CompareSnapshots(Snapshot snapshot1, Snapshot snapshot2)
        {
            SnapshotComparer comparer = new SnapshotComparer(snapshot1, snapshot2);
            comparer.Compare();
            return comparer;
        }

        private static string ExportToDiskIfRequested(SnapshotComparer comparer, CompareSnapshotsRequest request)
        {
            bool shouldExport = string.IsNullOrEmpty(request.ExportFileName);

            if (!shouldExport)
                return null;

            FileComparisonExporter exporter = new FileComparisonExporter
            {
                ResultsDirectory = request.ExportFileName
            };

            exporter.Export(comparer);

            return exporter.ExportDirectoryPath;
        }
    }
}