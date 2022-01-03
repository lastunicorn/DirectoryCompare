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

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using DustInTheWind.ConsoleFramework;
using DustInTheWind.DirectoryCompare.Application.SnapshotArea.CreateSnapshot;
using DustInTheWind.DirectoryCompare.Domain.DiskAnalysis;
using DustInTheWind.DirectoryCompare.Infrastructure;

namespace DustInTheWind.DirectoryCompare.Cli.UI.MiscellaneousCommands
{
    [Command("read")]
    [CommandDescription("Creates a new snapshot in a specific pot.")]
    public class CreateSnapshotCommand : ILongCommand
    {
        private readonly RequestBus requestBus;

        [CommandParameter(Index = 1)]
        public string PotName { get; set; }

        public event EventHandler<ProgressChangedEventArgs> Progress;

        public CreateSnapshotCommand(RequestBus requestBus)
        {
            this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
        }

        public async Task Execute(Arguments arguments)
        {
            CreateSnapshotRequest request = CreateRequest();

            IDiskAnalysisProgress diskAnalysisProgress = await requestBus.PlaceRequest<CreateSnapshotRequest, IDiskAnalysisProgress>(request);
            diskAnalysisProgress.Progress += HandleAnalysisProgress;

            diskAnalysisProgress.WaitToEnd();
        }

        private void HandleAnalysisProgress(object sender, DiskAnalysisProgressEventArgs value)
        {
            ProgressChangedEventArgs args = new((int)value.Percentage, null);
            OnProgress(args);
        }

        private CreateSnapshotRequest CreateRequest()
        {
            return new CreateSnapshotRequest
            {
                PotName = PotName
            };
        }

        protected virtual void OnProgress(ProgressChangedEventArgs e)
        {
            Progress?.Invoke(this, e);
        }
    }
}