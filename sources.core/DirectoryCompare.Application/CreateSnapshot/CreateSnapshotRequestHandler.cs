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
using System.Diagnostics;
using System.IO;
using DustInTheWind.DirectoryCompare.Domain;
using DustInTheWind.DirectoryCompare.Domain.DataAccess;
using DustInTheWind.DirectoryCompare.Domain.DiskAnalysis;
using DustInTheWind.DirectoryCompare.Domain.Logging;
using DustInTheWind.DirectoryCompare.JsonHashesFile.JsonExport;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Application.CreateSnapshot
{
    public class CreateSnapshotRequestHandler : RequestHandler<CreateSnapshotRequest>
    {
        private readonly IProjectLogger logger;
        private readonly IDiskAnalyzerFactory diskAnalyzerFactory;
        private readonly IPotRepository potRepository;
        private readonly IBlackListRepository blackListRepository;
        private readonly ISnapshotRepository snapshotRepository;

        public CreateSnapshotRequestHandler(IProjectLogger logger, IDiskAnalyzerFactory diskAnalyzerFactory,
            IPotRepository potRepository, IBlackListRepository blackListRepository, ISnapshotRepository snapshotRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.diskAnalyzerFactory = diskAnalyzerFactory ?? throw new ArgumentNullException(nameof(diskAnalyzerFactory));
            this.potRepository = potRepository ?? throw new ArgumentNullException(nameof(potRepository));
            this.blackListRepository = blackListRepository ?? throw new ArgumentNullException(nameof(blackListRepository));
            this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
        }

        protected override void Handle(CreateSnapshotRequest request)
        {
            Pot pot = potRepository.Get(request.PotName);

            if (pot == null)
                throw new Exception($"There is no pot with the name '{request.PotName}'.");

            logger.Info("Scanning path: {0}", pot.Path);

            using (Stream stream = snapshotRepository.CreateStream(pot.Name))
            using (StreamWriter streamWriter = new StreamWriter(stream))
            {
                AnalysisRequest analysisRequest = new AnalysisRequest
                {
                    RootPath = pot.Path,
                    BlackList = blackListRepository.Get(pot.Name)
                };
                JsonAnalysisExport jsonAnalysisExport = new JsonAnalysisExport(streamWriter);
                IDiskAnalyzer diskAnalyzer = diskAnalyzerFactory.Create(analysisRequest, jsonAnalysisExport);
                diskAnalyzer.ProgressIndicator = request.Progress;
                diskAnalyzer.Starting += HandleDiskReaderStarting;
                diskAnalyzer.ErrorEncountered += HandleDiskReaderErrorEncountered;

                Stopwatch stopwatch = Stopwatch.StartNew();
                diskAnalyzer.Run();
                stopwatch.Stop();

                logger.Info("Finished scanning path in {0}", stopwatch.Elapsed);
            }
        }

        private void HandleDiskReaderStarting(object sender, DiskReaderStartingEventArgs e)
        {
            if (e.BlackList.Count == 0)
            {
                logger.Info("No blacklist entries.");
                return;
            }

            logger.Info("Computed black list:");

            foreach (string blackListItem in e.BlackList)
                logger.Info("- " + blackListItem);
        }

        private void HandleDiskReaderErrorEncountered(object sender, ErrorEncounteredEventArgs e)
        {
            logger.Error("Error while reading path '{0}': {1}", e.Path, e.Exception);
        }
    }
}