// DirectoryCompare
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

using MediatR;

namespace DustInTheWind.Clindy.Applications.PresentDuplicates;

internal class PresentDuplicatesUseCase : IRequestHandler<PresentDuplicatesRequest, PresentDuplicatesResponse>
{
    private readonly ApplicationState applicationState;

    public PresentDuplicatesUseCase(ApplicationState applicationState)
    {
        this.applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));
    }

    public Task<PresentDuplicatesResponse> Handle(PresentDuplicatesRequest request, CancellationToken cancellationToken)
    {
        PresentDuplicatesResponse response = new()
        {
            Duplicates = applicationState.Duplicates.EnumerateOrdered(false)
                .ToList(),
            CurrentDuplicateGroup = applicationState.CurrentDuplicateGroup,
            DuplicateCount = applicationState.Duplicates.TotalDuplicatesCount,
            TotalSize = applicationState.Duplicates.TotalSize
        };

        return Task.FromResult(response);
    }
}