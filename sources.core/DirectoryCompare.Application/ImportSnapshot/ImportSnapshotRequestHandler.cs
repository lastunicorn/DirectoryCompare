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
using DustInTheWind.DirectoryCompare.Domain.DataAccess;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Domain.PotModel;
using DustInTheWind.DirectoryCompare.JsonHashesFile.Serialization;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Application.ImportSnapshot
{
    public class ImportSnapshotRequestHandler : RequestHandler<ImportSnapshotRequest>
    {
        private readonly IPotRepository potRepository;
        private readonly ISnapshotRepository snapshotRepository;

        public ImportSnapshotRequestHandler(IPotRepository potRepository, ISnapshotRepository snapshotRepository)
        {
            this.potRepository = potRepository ?? throw new ArgumentNullException(nameof(potRepository));
            this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
        }

        protected override void Handle(ImportSnapshotRequest request)
        {
            JSnapshotDocument jSnapshotDocument = JSnapshotDocument.Load(request.FilePath);
            Snapshot snapshot = jSnapshotDocument.Snapshot;

            Pot pot = potRepository.Get(request.PotName);

            if (pot == null)
            {
                pot = new Pot
                {
                    Name = request.PotName,
                    Path = snapshot.OriginalPath
                };

                potRepository.Add(pot);
            }
            else
            {
                if (pot.Path != snapshot.OriginalPath)
                    throw new Exception("The url of the imported snapshot is different than the one of the pot.");
            }

            snapshotRepository.Add(request.PotName, snapshot);
        }
    }
}