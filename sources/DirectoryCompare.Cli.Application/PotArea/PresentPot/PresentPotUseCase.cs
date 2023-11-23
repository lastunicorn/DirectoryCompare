// DirectoryCompare
// Copyright (C) 2017-2023 Dust in the Wind
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

namespace DustInTheWind.DirectoryCompare.Cli.Application.PotArea.PresentPot;

public class PresentPotUseCase : IRequestHandler<PresentPotRequest, PresentPotResponse>
{
    private readonly IPotRepository potRepository;

    public PresentPotUseCase(IPotRepository potRepository)
    {
        this.potRepository = potRepository ?? throw new ArgumentNullException(nameof(potRepository));
    }

    public async Task<PresentPotResponse> Handle(PresentPotRequest request, CancellationToken cancellationToken)
    {
        Pot pot = await RetrievePot(request);

        DataSize potSize = await potRepository.GetPotSize(pot.Guid);

        PresentPotResponse response = new()
        {
            Pot = new PotDto(pot)
            {
                Size = potSize
            }
        };

        return response;
    }

    private async Task<Pot> RetrievePot(PresentPotRequest request)
    {
        Pot pot = await potRepository.GetByNameOrId(request.PotName, includeSnapshots: true);

        if (pot == null)
            throw new PotNotFoundException(request.PotName);

        return pot;
    }
}