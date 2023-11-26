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

using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.DirectoryCompare.Cli.Application.MiscellaneousArea.CalculateHash;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.MiscellaneousCommands.Hash;

[NamedCommand("hash")]
internal class HashCommand : IConsoleCommand<HashViewModel>
{
    private readonly IMediator mediator;

    [NamedParameter("path", ShortName = 'p', Description = "The path of the file for which to calculate the hash.")]
    public string Path { get; set; }

    [NamedParameter("format", ShortName = 'f', IsOptional = true, Description = "The format in which the hash will be displayed.")]
    public BinaryDisplayFormat Format { get; set; } = BinaryDisplayFormat.Base64;

    public HashCommand(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }
    
    public async Task<HashViewModel> Execute()
    {
        CalculateHashRequest request = new()
        {
            Path = Path
        };

        CalculateHashResponse response = await mediator.Send(request);

        return new HashViewModel
        {
            Hash = response.Hash,
            Format = Format
        };
    }
}