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
using MediatR;
using System;
using DustInTheWind.DirectoryCompare.JsonHashesFile.Serialization;

namespace DustInTheWind.DirectoryCompare.Application.UseCases.Import
{
    public class ImportRequestHandler : RequestHandler<ImportRequest>
    {
        private readonly IPotRepository potRepository;
        private readonly ISnapshotRepository snapshotRepository;

        public ImportRequestHandler(IPotRepository potRepository, ISnapshotRepository snapshotRepository)
        {
            this.potRepository = potRepository ?? throw new ArgumentNullException(nameof(potRepository));
            this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
        }

        protected override void Handle(ImportRequest request)
        {
            SnapshotJsonFile snapshotJsonFile = SnapshotJsonFile.Load(request.FilePath);

            Pot pot = potRepository.Get(request.PotName);
            
            if (pot == null)
            {
                pot = new Pot
                {
                    Name = request.PotName,
                    Path = snapshotJsonFile.Snapshot.OriginalPath
                };

                potRepository.Add(pot);
            }
            else
            {
                if (pot.Path != snapshotJsonFile.Snapshot.OriginalPath)
                    throw new Exception("The url of the imported snapshot is different than the one of the pot.");
            }

            snapshotRepository.Add(request.PotName, snapshotJsonFile.Snapshot);
        }
    }
}