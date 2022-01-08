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
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DustInTheWind.DirectoryCompare.Application.MiscellaneousArea.CompareSnapshots;
using DustInTheWind.DirectoryCompare.Domain.Comparison;
using DustInTheWind.DirectoryCompare.Domain.DataAccess;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Application.MiscellaneousArea.CompareAllSnapshots
{
    public class CompareAllSnapshotsUseCase : IRequestHandler<CompareAllSnapshotsRequest, CompareAllSnapshotsResponse>
    {
        private readonly ISnapshotRepository snapshotRepository;
        private readonly DateTime executionTime = DateTime.UtcNow;

        public CompareAllSnapshotsUseCase(ISnapshotRepository snapshotRepository)
        {
            this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
        }

        public Task<CompareAllSnapshotsResponse> Handle(CompareAllSnapshotsRequest request, CancellationToken cancellationToken)
        {
            Snapshot[] snapshots = RetrieveAllSnapshots(request);

            for (int i = 0; i < snapshots.Length - 1; i++)
            {
                Snapshot currentSnapshot = snapshots[i];
                Snapshot previousSnapshot = snapshots[i + 1];

                SnapshotComparer comparer = CompareSnapshots(currentSnapshot, previousSnapshot);

                ExportToDisk(comparer, request.ExportName);
            }

            CompareAllSnapshotsResponse response = new();
            return Task.FromResult(response);
        }

        private Snapshot[] RetrieveAllSnapshots(CompareAllSnapshotsRequest request)
        {
            Snapshot[] snapshots = snapshotRepository.GetByPot(request.PotName)
                .OrderByDescending(x => x.CreationTime)
                .ToArray();
            return snapshots;
        }

        private static SnapshotComparer CompareSnapshots(Snapshot snapshot1, Snapshot snapshot2)
        {
            SnapshotComparer comparer = new(snapshot1, snapshot2);
            comparer.Compare();

            return comparer;
        }

        private void ExportToDisk(SnapshotComparer comparer, string exportName)
        {
            FileComparisonExporter exporter = new()
            {
                ExportName = CalculateExportDirectoryPath(comparer, exportName)
            };

            exporter.Export(comparer);
        }

        private string CalculateExportDirectoryPath(SnapshotComparer comparer, string exportName)
        {
            string exportDirectoryNameBase = $"{exportName} - {executionTime:yyyy MM dd HHmmss}";
            string exportDirectoryName = $"{comparer.Snapshot1.CreationTime:yyyy MM dd HHmmss}";
            string exportDirectoryPath = Path.Combine(exportDirectoryNameBase, exportDirectoryName);
            
            return exportDirectoryPath;
        }
    }
}