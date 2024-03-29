﻿// DirectoryCompare
// Copyright (C) 2017-2024 Dust in the Wind
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

using DustInTheWind.DirectoryCompare.DataStructures;
using DustInTheWind.DirectoryCompare.Domain.PotModel;
using DustInTheWind.DirectoryCompare.Ports.DataAccess;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Cli.Application.PotArea.PresentPots;

public class PresentPotsUseCase : IRequestHandler<PresentPotsRequest, PresentPotsResponse>
{
    private readonly IPotRepository potRepository;

    public PresentPotsUseCase(IPotRepository potRepository)
    {
        this.potRepository = potRepository ?? throw new ArgumentNullException(nameof(potRepository));
    }

    public async Task<PresentPotsResponse> Handle(PresentPotsRequest request, CancellationToken cancellationToken)
    {
        IEnumerable<Pot> pots = (await potRepository.GetAll())
            .OrderBy(x => x.Name);

        DataSize totalSize = DataSize.Zero;
        List<PotDto> potsDto = new();

        foreach (Pot pot in pots)
        {
            DataSize potSize = await potRepository.GetPotSize(pot.Guid);
            totalSize += potSize;
            
            PotDto potDto = new(pot)
            {
                Size = potSize
            };
            potsDto.Add(potDto);
        }

        return new PresentPotsResponse
        {
            TotalSize = totalSize,
            Pots = potsDto
        };
    }
}