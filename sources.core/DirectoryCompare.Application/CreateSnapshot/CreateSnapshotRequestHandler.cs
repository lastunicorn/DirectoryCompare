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
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using DustInTheWind.ConsoleFramework.Logging;
using DustInTheWind.DirectoryCompare.Domain.DataAccess;
using DustInTheWind.DirectoryCompare.Domain.DiskAnalysis;
using DustInTheWind.DirectoryCompare.Domain.PotModel;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Application.CreateSnapshot
{
    public class CreateSnapshotRequestHandler : RequestHandler<CreateSnapshotRequest, SnapshotProgress>
    {
        private readonly IProjectLogger logger;
        private readonly IPotRepository potRepository;
        private readonly IBlackListRepository blackListRepository;
        private readonly ISnapshotRepository snapshotRepository;

        public CreateSnapshotRequestHandler(IProjectLogger logger, IPotRepository potRepository,
            IBlackListRepository blackListRepository, ISnapshotRepository snapshotRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.potRepository = potRepository ?? throw new ArgumentNullException(nameof(potRepository));
            this.blackListRepository = blackListRepository ?? throw new ArgumentNullException(nameof(blackListRepository));
            this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
        }

        protected override SnapshotProgress Handle(CreateSnapshotRequest request)
        {
            Pot pot = potRepository.Get(request.PotName);

            if (pot == null)
                throw new PotDoesNotExistException(request.PotName);

            return StartAnalysis(pot);
        }

        private SnapshotProgress StartAnalysis(Pot pot)
        {
            SnapshotProgress snapshotProgress = new SnapshotProgress();

            Task.Run(() => { RunAnalysis(pot, snapshotProgress); });

            return snapshotProgress;
        }

        private void RunAnalysis(Pot pot, SnapshotProgress snapshotProgress)
        {
            logger.WriteInfo("Scanning path: {0}", pot.Path);

            using (Stream stream = snapshotRepository.CreateStream(pot.Name))
            using (StreamWriter streamWriter = new StreamWriter(stream))
            {
                DiskAnalysis diskAnalysis = new DiskAnalysis
                {
                    RootPath = pot.Path,
                    AnalysisExport = new JsonAnalysisExport(streamWriter),
                    BlackList = blackListRepository.Get(pot.Name)
                };

                diskAnalysis.Starting += HandleDiskReaderStarting;
                diskAnalysis.ErrorEncountered += HandleDiskReaderErrorEncountered;
                diskAnalysis.Progress += (sender, args) => snapshotProgress.ReportProgress(args.Percentage);

                Stopwatch stopwatch = Stopwatch.StartNew();
                diskAnalysis.Run();
                stopwatch.Stop();

                snapshotProgress.ReportEnd();

                logger.WriteInfo("Finished scanning path in {0}", stopwatch.Elapsed);
            }
        }

        private void HandleDiskReaderStarting(object sender, DiskReaderStartingEventArgs e)
        {
            if (e.BlackList.Count == 0)
            {
                logger.WriteInfo("No blacklist entries.");
                return;
            }

            logger.WriteInfo("Computed black list:");

            foreach (string blackListItem in e.BlackList)
                logger.WriteInfo("- " + blackListItem);
        }

        private void HandleDiskReaderErrorEncountered(object sender, ErrorEncounteredEventArgs e)
        {
            logger.WriteError("Error while reading path '{0}': {1}", e.Path, e.Exception);
        }
    }
}