// Directory Compare
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

using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.DirectoryCompare.Cli.Application.MiscellaneousArea.FindChange;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.MiscellaneousCommands.FindChange;

[NamedCommand("find-change", Description = "Checks in which snapshot the file was changed.")]
internal class FindChangeCommand : IConsoleCommand<FindChangeViewModel>
{
    private readonly IMediator mediator;

    [AnonymousParameter(Order = 1, Description = "The location of the snapshot containing the file to be checked. Location structure: '<pot>~<snapshot>:<path>'.")]
    public string SnapshotLocation { get; set; }

    public FindChangeCommand(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<FindChangeViewModel> Execute()
    {
        FindChangeRequest request = new()
        {
            SnapshotLocation = SnapshotLocation
        };
        FindChangeResponse response = await mediator.Send(request);

        return new FindChangeViewModel
        {
            Path = response.Path,
            Changes = response.Changes
        };
    }
}