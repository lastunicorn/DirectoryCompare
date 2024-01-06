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

using DustInTheWind.DirectoryCompare.Cli.Application.SnapshotArea.PresentSnapshot;
using DustInTheWind.DirectoryCompare.DataStructures;
using DustInTheWind.DirectoryCompare.Domain.Comparison;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Ports.DataAccess;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Cli.Application.MiscellaneousArea.CompareSnapshots;

public class CompareSnapshotsUseCase : IRequestHandler<CompareSnapshotsRequest, CompareSnapshotsResponse>
{
    private readonly ISnapshotRepository snapshotRepository;

    public CompareSnapshotsUseCase(ISnapshotRepository snapshotRepository)
    {
        this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
    }

    public async Task<CompareSnapshotsResponse> Handle(CompareSnapshotsRequest request, CancellationToken cancellationToken)
    {
        Snapshot snapshot1 = await RetrieveSnapshot(request.Snapshot1);
        Snapshot snapshot2 = await RetrieveSnapshot(request.Snapshot2);

        SnapshotComparison comparison = CompareSnapshots(snapshot1, request.Snapshot1.InternalPath, snapshot2, request.Snapshot2.InternalPath);
        string exportDirectoryPath = ExportToDiskIfRequested(comparison, request);

        return new CompareSnapshotsResponse
        {
            OnlyInSnapshot1 = comparison.OnlyInSnapshot1,
            OnlyInSnapshot2 = comparison.OnlyInSnapshot2,
            DifferentNames = comparison.DifferentNames.ToDto(),
            DifferentContent = comparison.DifferentContent.ToDto(),
            ExportDirectoryPath = exportDirectoryPath
        };
    }

    private async Task<Snapshot> RetrieveSnapshot(SnapshotLocation snapshotLocation)
    {
        Snapshot snapshot = await snapshotRepository.Get(snapshotLocation);

        if (snapshot == null)
            throw new SnapshotNotFoundException(snapshotLocation);
        return snapshot;
    }

    private static SnapshotComparison CompareSnapshots(Snapshot snapshot1, SnapshotPath snapshotPath1, Snapshot snapshot2, SnapshotPath snapshotPath2)
    {
        SnapshotComparison comparison = new(snapshot1, snapshot2)
        {
            Path1 = snapshotPath1,
            Path2 = snapshotPath2
        };
        comparison.Compare();
        return comparison;
    }

    private static string ExportToDiskIfRequested(SnapshotComparison comparison, CompareSnapshotsRequest request)
    {
        return request.IsExportRequested
            ? ExportToDisk(comparison, request.ExportFileName)
            : null;
    }

    private static string ExportToDisk(SnapshotComparison comparison, string exportFileName)
    {
        FileComparisonExporter exporter = new()
        {
            ExportName = exportFileName,
            AddTimeStamp = true
        };

        exporter.Export(comparison);

        return exporter.ExportDirectoryPath;
    }
}