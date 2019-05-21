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
using DustInTheWind.DirectoryCompare.DiskAnalysis;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Application.Compare
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
            AnalysisRequest analysisRequest1 = new AnalysisRequest
            {
                RootPath = request.Path1
            };
            SnapshotDiskAnalysisExport snapshotDiskAnalysisExport1 = new SnapshotDiskAnalysisExport();
            IDiskAnalyzer diskReader1 = diskAnalyzerFactory.Create(analysisRequest1, snapshotDiskAnalysisExport1);
            diskReader1.Starting += HandleDiskReaderStarting;
            diskReader1.Read();

            AnalysisRequest analysisRequest2 = new AnalysisRequest
            {
                RootPath = request.Path2
            };
            SnapshotDiskAnalysisExport snapshotDiskAnalysisExport2 = new SnapshotDiskAnalysisExport();
            IDiskAnalyzer diskReader2 = diskAnalyzerFactory.Create(analysisRequest2, snapshotDiskAnalysisExport2);
            diskReader2.Starting += HandleDiskReaderStarting;
            diskReader2.Read();

            SnapshotComparer comparer = new SnapshotComparer(snapshotDiskAnalysisExport1.Snapshot, snapshotDiskAnalysisExport2.Snapshot);
            comparer.Compare();

            request.Exporter.Export(comparer);
        }

        private static void HandleDiskReaderStarting(object sender, DiskReaderStartingEventArgs e)
        {
            Console.WriteLine("Computed black list:");

            foreach (string blackListItem in e.BlackList)
                Console.WriteLine("- " + blackListItem);
        }
    }
}