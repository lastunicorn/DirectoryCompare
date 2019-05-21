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
using DustInTheWind.DirectoryCompare.Comparison;
using DustInTheWind.DirectoryCompare.DiskAnalysis;
using DustInTheWind.DirectoryCompare.Entities;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Application.Comparison
{
    public class ComparePathsRequestHandler : RequestHandler<ComparePathsRequest>
    {
        private readonly IDiskAnalyzerFactory diskAnalyzerFactory;

        public ComparePathsRequestHandler(IDiskAnalyzerFactory diskAnalyzerFactory)
        {
            this.diskAnalyzerFactory = diskAnalyzerFactory ?? throw new ArgumentNullException(nameof(diskAnalyzerFactory));
        }

        protected override void Handle(ComparePathsRequest request)
        {
            Snapshot snapshot1 = ReadPath(request.Path1);
            Snapshot snapshot2 = ReadPath(request.Path2);

            SnapshotComparer comparer = new SnapshotComparer(snapshot1, snapshot2);
            comparer.Compare();

            request.Exporter.Export(comparer);
        }

        private Snapshot ReadPath(string path)
        {
            AnalysisRequest analysisRequest = new AnalysisRequest
            {
                RootPath = path
            };

            SnapshotDiskAnalysisExport snapshotDiskAnalysisExport = new SnapshotDiskAnalysisExport();
            IDiskAnalyzer diskReader = diskAnalyzerFactory.Create(analysisRequest, snapshotDiskAnalysisExport);
            diskReader.Starting += HandleDiskReaderStarting;
            diskReader.Read();

            return snapshotDiskAnalysisExport.Snapshot;
        }

        private static void HandleDiskReaderStarting(object sender, DiskReaderStartingEventArgs e)
        {
            Console.WriteLine("Computed black list:");

            foreach (string blackListItem in e.BlackList)
                Console.WriteLine("- " + blackListItem);
        }
    }
}