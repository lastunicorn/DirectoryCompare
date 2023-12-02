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
using DustInTheWind.DirectoryCompare.Ports.UserAccess;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Cli.Application.PotArea.DeletePot;

public class DeletePotUseCase : IRequestHandler<DeletePotRequest>
{
    private readonly IPotRepository potRepository;
    private readonly IDeletePotUi deletePotUi;

    public DeletePotUseCase(IPotRepository potRepository, IDeletePotUi deletePotUi)
    {
        this.potRepository = potRepository ?? throw new ArgumentNullException(nameof(potRepository));
        this.deletePotUi = deletePotUi ?? throw new ArgumentNullException(nameof(deletePotUi));
    }

    public async Task Handle(DeletePotRequest request, CancellationToken cancellationToken)
    {
        Pot pot = await RetrievePot(request.PotName);
        await AskForConfirmation(pot);
        await potRepository.DeleteById(pot.Guid);
    }

    private async Task<Pot> RetrievePot(string nameOrId)
    {
        Pot pot = await potRepository.GetByNameOrId(nameOrId);

        if (pot == null)
            throw new PotNotFoundException(nameOrId);

        return pot;
    }

    private async Task AskForConfirmation(Pot pot)
    {
        PotDeletionRequest potDeletionRequest = new()
        {
            PotName = pot.Name,
            PotId = pot.Guid
        };
        bool confirmation = await deletePotUi.ConfirmToDelete(potDeletionRequest);

        if (!confirmation)
            throw new OperationCanceledException($"The pot {pot.Name} was not deleted.");
    }
}