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
using DustInTheWind.DirectoryCompare.Cli.Application.MiscellaneousArea.Purge;
using DustInTheWind.DirectoryCompare.DataStructures;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.MiscellaneousCommands.Purge;

[NamedCommand("purge", Description = "Removes a file or directory from the specified snapshot.")]
public class PurgeCommand : IConsoleCommand
{
    private readonly IMediator mediator;

    [AnonymousParameter(Order = 1, Description = "The snapshot from which to remove the file.")]
    public SnapshotLocation Snapshot { get; set; }

    [AnonymousParameter(Order = 2, Description = "The full path of the file or directory to purge.")]
    public string FilePath { get; set; }

    public PurgeCommand(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task Execute()
    {
        PurgeRequest request = new()
        {
            Snapshot = Snapshot,
            FilePath = FilePath
        };

        PurgeResponse response = await mediator.Send(request);

        foreach (string message in response.Log)
            Console.WriteLine(message);
    }
}