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
using DustInTheWind.DirectoryCompare.DiskAnalysis;
using DustInTheWind.DirectoryCompare.Domain;
using DustInTheWind.DirectoryCompare.Infrastructure;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.SnapshotCommands
{
    // Example:
    // snapshot -c <pot-name>
    // snapshot --create <pot-name>
    
    [Command("snapshot")]
    [CommandDescription("Creates a new snapshot in a specific pot.")]
    public class CreateSnapshotCommandModel : ILongCommandModel
    {
        private readonly RequestBus requestBus;

        [CommandParameter(ShortName = "c", LongName = "create")]
        public string PotName { get; set; }

        public event EventHandler<ProgressChangedEventArgs> Progress;

        public SnapshotLocation SnapshotLocation { get; set; }
        
        public CreateSnapshotCommandModel(RequestBus requestBus)
        {
            this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
        }

        public async Task Execute(Arguments arguments)
        {
            CreateSnapshotRequest request = CreateRequest();

            IDiskAnalysisProgress diskAnalysisProgress = await requestBus.PlaceRequest<CreateSnapshotRequest, IDiskAnalysisProgress>(request);
            diskAnalysisProgress.Progress += HandleAnalysisProgress;

            diskAnalysisProgress.WaitToEnd();

            SnapshotLocation = "missing";
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