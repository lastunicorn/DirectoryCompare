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
using DustInTheWind.DirectoryCompare.Cli.Application;
using DustInTheWind.DirectoryCompare.Cli.Application.SnapshotArea.CreateSnapshot;
using DustInTheWind.DirectoryCompare.Cli.Application.SnapshotArea.CreateSnapshot.DiskAnalysis;
using DustInTheWind.DirectoryCompare.Cli.Presentation.SnapshotCommands.CreateSnapshot;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.SnapshotCommands.ReadSnapshot;

// Example:
// read <pot-name>

[NamedCommand("read", Description = "Creates a new snapshot in a specific pot. This is an alias for the create-snapshot command.")]
[CommandOrder(9)]
public class ReadSnapshotCommand : IConsoleCommand
{
    private readonly RequestBus requestBus;
    private readonly CreateSnapshotCommandView view;

    [AnonymousParameter(Order = 1, Description = "The name or id of the pot for which to create a new snapshot.")]
    public string PotName { get; set; }

    public ReadSnapshotCommand(RequestBus requestBus)
    {
        this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));

        view = new CreateSnapshotCommandView();
    }

    public async Task Execute()
    {
        CreateSnapshotRequest request = new()
        {
            PotName = PotName
        };

        IDiskAnalysisProgress diskAnalysisProgress = await requestBus.PlaceRequest<CreateSnapshotRequest, IDiskAnalysisProgress>(request);
        diskAnalysisProgress.Progress += HandleAnalysisProgress;

        diskAnalysisProgress.WaitToEnd();
        view.FinishDisplay();
    }

    private void HandleAnalysisProgress(object sender, DiskAnalysisProgressEventArgs e)
    {
        view.HandleProgress(e);
    }
}