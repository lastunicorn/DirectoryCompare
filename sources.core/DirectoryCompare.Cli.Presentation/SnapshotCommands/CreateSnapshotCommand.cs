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

using System.ComponentModel;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.DirectoryCompare.Cli.Application.SnapshotArea.CreateSnapshot;
using DustInTheWind.DirectoryCompare.DiskAnalysis;
using DustInTheWind.DirectoryCompare.Domain;
using DustInTheWind.DirectoryCompare.Infrastructure;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.SnapshotCommands;

// Example:
// snapshot -c <pot-name>
// snapshot --create <pot-name>

[NamedCommand("create-snapshot", Order = 5, Description = "Creates a new snapshot in a specific pot.")]
public class CreateSnapshotCommand : CommandBase<CreateSnapshotCommandView>
{
    private readonly RequestBus requestBus;

    [NamedParameter("pot", ShortName = 'p')]
    public string PotName { get; set; }

    public CreateSnapshotCommand(RequestBus requestBus)
        : base(new CreateSnapshotCommandView())
    {
        this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
    }

    public override async Task Execute()
    {
        CreateSnapshotRequest request = new()
        {
            PotName = PotName
        };

        IDiskAnalysisProgress diskAnalysisProgress = await requestBus.PlaceRequest<CreateSnapshotRequest, IDiskAnalysisProgress>(request);
        diskAnalysisProgress.Progress += HandleAnalysisProgress;

        diskAnalysisProgress.WaitToEnd();
        Console.FinishDisplay();
    }

    private void HandleAnalysisProgress(object sender, DiskAnalysisProgressEventArgs value)
    {
        int percentage = (int)value.Percentage;
        Console.HandleProgress(percentage);
    }
}