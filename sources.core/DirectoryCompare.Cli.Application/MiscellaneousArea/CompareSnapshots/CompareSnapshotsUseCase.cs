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

using DustInTheWind.DirectoryCompare.Domain.Comparison;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Cli.Application.MiscellaneousArea.CompareSnapshots;

public class CompareSnapshotsUseCase : IRequestHandler<CompareSnapshotsRequest, CompareSnapshotsResponse>
{
    private readonly SnapshotFactory snapshotFactory;

    public CompareSnapshotsUseCase(SnapshotFactory snapshotFactory)
    {
        this.snapshotFactory = snapshotFactory ?? throw new ArgumentNullException(nameof(snapshotFactory));
    }

    public Task<CompareSnapshotsResponse> Handle(CompareSnapshotsRequest request, CancellationToken cancellationToken)
    {
        Snapshot snapshot1 = snapshotFactory.RetrieveSnapshot(request.Snapshot1);
        Snapshot snapshot2 = snapshotFactory.RetrieveSnapshot(request.Snapshot2);
        SnapshotComparison comparison = CompareSnapshots(snapshot1, snapshot2);
        string exportDirectoryPath = ExportToDiskIfRequested(comparison, request);

        CompareSnapshotsResponse response = new ()
        {
            OnlyInSnapshot1 = comparison.OnlyInSnapshot1,
            OnlyInSnapshot2 = comparison.OnlyInSnapshot2,
            DifferentNames = comparison.DifferentNames,
            DifferentContent = comparison.DifferentContent,
            ExportDirectoryPath = exportDirectoryPath
        };

        return Task.FromResult(response);
    }

    private static SnapshotComparison CompareSnapshots(Snapshot snapshot1, Snapshot snapshot2)
    {
        SnapshotComparison comparison = new(snapshot1, snapshot2);
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