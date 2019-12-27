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

using DustInTheWind.DirectoryCompare.Domain;
using DustInTheWind.DirectoryCompare.Domain.DataAccess;
using DustInTheWind.DirectoryCompare.Domain.DiskAnalysis;
using DustInTheWind.DirectoryCompare.Domain.Logging;
using DustInTheWind.DirectoryCompare.Domain.Utils;
using MediatR;
using System;
using System.Diagnostics;
using System.IO;

namespace DustInTheWind.DirectoryCompare.Application.UseCases.CreateSnapshot
{
    public class CreateSnapshotRequestHandler : RequestHandler<CreateSnapshotRequest>
    {
        private readonly IProjectLogger logger;
        private readonly IDiskAnalyzerFactory diskAnalyzerFactory;
        private readonly IAnalysisExportFactory analysisExportFactory;
        private readonly IPotRepository potRepository;
        private readonly IBlackListRepository blackListRepository;
        private readonly ISnapshotRepository snapshotRepository;

        public CreateSnapshotRequestHandler(IProjectLogger logger, IDiskAnalyzerFactory diskAnalyzerFactory, IAnalysisExportFactory analysisExportFactory,
            IPotRepository potRepository, IBlackListRepository blackListRepository, ISnapshotRepository snapshotRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.diskAnalyzerFactory = diskAnalyzerFactory ?? throw new ArgumentNullException(nameof(diskAnalyzerFactory));
            this.analysisExportFactory = analysisExportFactory ?? throw new ArgumentNullException(nameof(analysisExportFactory));
            this.potRepository = potRepository ?? throw new ArgumentNullException(nameof(potRepository));
            this.blackListRepository = blackListRepository ?? throw new ArgumentNullException(nameof(blackListRepository));
            this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
        }

        protected override void Handle(CreateSnapshotRequest request)
        {
            Pot pot = potRepository.Get(request.PotName);

            if (pot == null)
                throw new Exception("There is no pot with the specified name");

            PathCollection blackList = blackListRepository.Get(pot.Name);

            logger.Info("Scanning path: {0}", pot.Path);
            logger.Info("Black List:");

            if (blackList != null)
                foreach (string blackListItem in blackList)
                    logger.Info(blackListItem);

            using (Stream stream = snapshotRepository.CreateStream(pot.Name))
            using (StreamWriter streamWriter = new StreamWriter(stream))
            {
                AnalysisRequest analysisRequest = new AnalysisRequest
                {
                    RootPath = pot.Path,
                    BlackList = blackList
                };
                IAnalysisExport jsonAnalysisExport = analysisExportFactory.Create(streamWriter);
                IDiskAnalyzer diskAnalyzer = diskAnalyzerFactory.Create(analysisRequest, jsonAnalysisExport);
                diskAnalyzer.Starting += HandleDiskReaderStarting;
                diskAnalyzer.ErrorEncountered += HandleDiskReaderErrorEncountered;

                Stopwatch stopwatch = Stopwatch.StartNew();
                diskAnalyzer.Run();
                stopwatch.Stop();

                logger.Info("Finished scanning path {0}", stopwatch.Elapsed);
            }
        }

        private static void HandleDiskReaderStarting(object sender, DiskReaderStartingEventArgs e)
        {
            Console.WriteLine("Computed black list:");

            foreach (string blackListItem in e.BlackList)
                Console.WriteLine("- " + blackListItem);
        }

        private void HandleDiskReaderErrorEncountered(object sender, ErrorEncounteredEventArgs e)
        {
            logger.Error("Error while reading path '{0}': {1}", e.Path, e.Exception);
        }
    }
}