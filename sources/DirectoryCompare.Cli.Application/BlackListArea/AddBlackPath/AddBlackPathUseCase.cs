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

using DustInTheWind.DirectoryCompare.Ports.DataAccess;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Cli.Application.BlackListArea.AddBlackPath;

public class AddBlackPathUseCase : IRequestHandler<AddBlackPathRequest>
{
    private readonly IBlackListRepository blackListRepository;

    public AddBlackPathUseCase(IBlackListRepository blackListRepository)
    {
        this.blackListRepository = blackListRepository ?? throw new ArgumentNullException(nameof(blackListRepository));
    }

    public Task Handle(AddBlackPathRequest request, CancellationToken cancellationToken)
    {
        if (request.Path.IsEmpty)
            throw new Exception("Path was not provided.");

        return blackListRepository.Add(request.PotName, request.Path);
    }
}