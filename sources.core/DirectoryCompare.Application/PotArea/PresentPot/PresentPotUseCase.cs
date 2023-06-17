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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DustInTheWind.DirectoryCompare.Domain.DataAccess;
using DustInTheWind.DirectoryCompare.Domain.PotModel;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Application.PotArea.PresentPot;

public class PresentPotUseCase : IRequestHandler<PresentPotRequest, Pot>
{
    private readonly IPotRepository potRepository;
    private readonly ISnapshotRepository snapshotRepository;

    public PresentPotUseCase(IPotRepository potRepository, ISnapshotRepository snapshotRepository)
    {
        this.potRepository = potRepository ?? throw new ArgumentNullException(nameof(potRepository));
        this.snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
    }

    public Task<Pot> Handle(PresentPotRequest request, CancellationToken cancellationToken)
    {
        Pot pot = potRepository.Get(request.PotName);
        pot.Snapshots = snapshotRepository.GetByPot(request.PotName).ToList();

        return Task.FromResult(pot);
    }
}