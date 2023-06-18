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

using DustInTheWind.DirectoryCompare.Domain.Utils;
using DustInTheWind.DirectoryCompare.Ports.DataAccess;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Cli.Application.BlackListArea.PresentBlackList;

public class PresentBlackListUseCase : IRequestHandler<PresentBlackListRequest, DiskPathCollection>
{
    private readonly IBlackListRepository blackListRepository;

    public PresentBlackListUseCase(IBlackListRepository blackListRepository)
    {
        this.blackListRepository = blackListRepository ?? throw new ArgumentNullException(nameof(blackListRepository));
    }

    public Task<DiskPathCollection> Handle(PresentBlackListRequest request, CancellationToken cancellationToken)
    {
        DiskPathCollection diskPathCollection = blackListRepository.Get(request.PotName);

        return Task.FromResult(diskPathCollection);
    }
}