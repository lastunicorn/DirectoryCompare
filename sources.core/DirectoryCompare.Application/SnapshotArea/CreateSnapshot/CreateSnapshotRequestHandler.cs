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
using System.Threading.Tasks;
using DustInTheWind.DirectoryCompare.Domain.DataAccess;
using DustInTheWind.DirectoryCompare.Domain.DiskAnalysis;
using DustInTheWind.DirectoryCompare.Domain.ImportExport;
using DustInTheWind.DirectoryCompare.Domain.Logging;
using DustInTheWind.DirectoryCompare.Domain.PotModel;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Application.SnapshotArea.CreateSnapshot
{
    public class CreateSnapshotRequestHandler : RequestHandler<CreateSnapshotRequest, IDiskAnalysisProgress>
    {
        private readonly ILog log;
        private readonly IPotRepository potRepository;
        private readonly IBlackListRepository blackListRepository;
        private readonly ISnapshotRepository snapshotRepository;

        public CreateSnapshotRequestHandler(ILog log, IPotRepository potRepository,
            IBlackListRepository blackListRepository, ISnapshotRepository snapshotRepository)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));
            this.potRepository = potRepository ?? throw new ArgumentNullException(nameof(potRepository));
            this.blackListRepository = blackListRepository ?? throw new ArgumentNullException(nameof(blackListRepository));
            this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
        }

        protected override IDiskAnalysisProgress Handle(CreateSnapshotRequest request)
        {
            Pot pot = RetrievePot(request);
            return StartPathAnalysis2(pot);
        }

        private Pot RetrievePot(CreateSnapshotRequest request)
        {
            Pot pot = potRepository.Get(request.PotName);

            if (pot == null)
                throw new PotDoesNotExistException(request.PotName);

            return pot;
        }

        private DiskAnalysis StartPathAnalysis2(Pot pot)
        {
            log.WriteInfo("Scanning path: {0}", pot.Path);

            ISnapshotWriter snapshotWriter = snapshotRepository.CreateWriter(pot.Name);

            DiskAnalysis diskAnalysis = new DiskAnalysis
            {
                RootPath = pot.Path,
                SnapshotWriter = snapshotWriter,
                BlackList = blackListRepository.Get(pot.Name)
            };

            diskAnalysis.Starting += HandleDiskReaderStarting;
            diskAnalysis.ErrorEncountered += HandleDiskReaderErrorEncountered;

            Task.Run(() =>
            {
                try
                {
                    diskAnalysis.Run();
                }
                finally
                {
                    log.WriteInfo("Finished scanning path in {0}", diskAnalysis.ElapsedTime);
                    snapshotWriter.Dispose();
                }
            });

            return diskAnalysis;
        }

        private void HandleDiskReaderStarting(object sender, DiskReaderStartingEventArgs e)
        {
            if (e.BlackList.Count == 0)
            {
                log.WriteInfo("No blacklist entries.");
                return;
            }

            log.WriteInfo("Computed black list:");

            foreach (string blackListItem in e.BlackList)
                log.WriteInfo("- " + blackListItem);
        }

        private void HandleDiskReaderErrorEncountered(object sender, ErrorEncounteredEventArgs e)
        {
            log.WriteError("Error while reading path '{0}': {1}", e.Path, e.Exception);
        }
    }
}