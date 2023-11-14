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

using DustInTheWind.DirectoryCompare.Domain.PotModel;
using DustInTheWind.DirectoryCompare.Ports.DataAccess;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Cli.Application.PotArea.CreatePot;

public class CreatePotUseCase : IRequestHandler<CreatePotRequest>
{
    private readonly IPotRepository potRepository;

    public CreatePotUseCase(IPotRepository potRepository)
    {
        this.potRepository = potRepository ?? throw new ArgumentNullException(nameof(potRepository));
    }

    public async Task Handle(CreatePotRequest request, CancellationToken cancellationToken)
    {
        await VerifyPotDoesNotExist(request.Name);
        await CreateNewPot(request);
    }

    private async Task VerifyPotDoesNotExist(string potName)
    {
        bool potAlreadyExists = await potRepository.ExistsByName(potName);

        if (potAlreadyExists)
            throw new PotAlreadyExistsException();
    }

    private async Task CreateNewPot(CreatePotRequest request)
    {
        Pot newPot = new()
        {
            Name = request.Name,
            Path = request.Path
        };

        await potRepository.Add(newPot);
    }
}