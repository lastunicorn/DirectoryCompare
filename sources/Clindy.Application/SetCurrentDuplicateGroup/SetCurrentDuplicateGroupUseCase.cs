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

using DustInTheWind.DirectoryCompare.Ports.ImportExportAccess;
using MediatR;

namespace DustInTheWind.Clindy.Applications.SetCurrentDuplicateGroup;

internal class SetCurrentDuplicateGroupUseCase : IRequestHandler<SetCurrentDuplicateGroupRequest>
{
    private readonly ApplicationState applicationState;
    private readonly EventBus eventBus;

    public SetCurrentDuplicateGroupUseCase(ApplicationState applicationState, EventBus eventBus)
    {
        this.applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));
        this.eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }

    public Task Handle(SetCurrentDuplicateGroupRequest request, CancellationToken cancellationToken)
    {
        FileDuplicateGroup fileDuplicateGroup = applicationState.Duplicates
            .FirstOrDefault(x => x.FileHash == request.Hash);
        
        applicationState.CurrentDuplicateGroup = fileDuplicateGroup;

        CurrentDuplicateReplacedEvent currentDuplicateReplaced = new()
        {
            DuplicateGroup = fileDuplicateGroup
        };
        return eventBus.PublishAsync(currentDuplicateReplaced, cancellationToken);
    }
}