// DirectoryCompare
// Copyright (C) 2017-2024 Dust in the Wind
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

using DustInTheWind.DirectoryCompare.Cli.Application.MiscellaneousArea.CompareSnapshots;
using DustInTheWind.DirectoryCompare.Domain.Comparison;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Ports.DataAccess;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Cli.Application.MiscellaneousArea.CompareAllSnapshots;

public class CompareAllSnapshotsUseCase : IRequestHandler<CompareAllSnapshotsRequest, CompareAllSnapshotsResponse>
{
    private readonly ISnapshotRepository snapshotRepository;
    private readonly DateTime executionTime = DateTime.UtcNow;

    public CompareAllSnapshotsUseCase(ISnapshotRepository snapshotRepository)
    {
        this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
    }

    public async Task<CompareAllSnapshotsResponse> Handle(CompareAllSnapshotsRequest request, CancellationToken cancellationToken)
    {
        Snapshot[] snapshots = await RetrieveAllSnapshots(request);

        for (int i = 0; i < snapshots.Length - 1; i++)
        {
            Snapshot currentSnapshot = snapshots[i];
            Snapshot previousSnapshot = snapshots[i + 1];

            SnapshotComparison comparison = CompareSnapshots(currentSnapshot, previousSnapshot);

            ExportToDisk(comparison, request.ExportName);
        }

        CompareAllSnapshotsResponse response = new();
        return response;
    }

    private async Task<Snapshot[]> RetrieveAllSnapshots(CompareAllSnapshotsRequest request)
    {
        Snapshot[] snapshots = await snapshotRepository.GetByPot(request.PotName)
            .OrderByDescending(x => x.CreationTime)
            .ToArrayAsync();
        return snapshots;
    }

    private static SnapshotComparison CompareSnapshots(Snapshot snapshot1, Snapshot snapshot2)
    {
        SnapshotComparison comparison = new(snapshot1, snapshot2);
        comparison.Compare();

        return comparison;
    }

    private void ExportToDisk(SnapshotComparison comparison, string exportName)
    {
        FileComparisonExporter exporter = new()
        {
            ExportName = CalculateExportDirectoryPath(comparison, exportName)
        };

        exporter.Export(comparison);
    }

    private string CalculateExportDirectoryPath(SnapshotComparison comparison, string exportName)
    {
        string exportDirectoryNameBase = $"{exportName} - {executionTime:yyyy MM dd HHmmss}";
        string exportDirectoryName = $"{comparison.Snapshot1.CreationTime:yyyy MM dd HHmmss}";
        string exportDirectoryPath = Path.Combine(exportDirectoryNameBase, exportDirectoryName);

        return exportDirectoryPath;
    }
}