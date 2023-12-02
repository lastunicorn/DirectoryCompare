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

using System.Diagnostics;
using DustInTheWind.DirectoryCompare.DataStructures;
using DustInTheWind.DirectoryCompare.Domain.Utils;
using DustInTheWind.DirectoryCompare.Ports.UserAccess;

namespace DustInTheWind.DirectoryCompare.Cli.Application.SnapshotArea.CreateSnapshot.DiskAnalysis;

internal class AnalysisProgress
{
    private readonly ICreateSnapshotUi createSnapshotUi;
    private DataSizeProgress progress;
    private float lastPercentageAnnounced;
    private readonly Stopwatch stopwatch = new();

    public TimeSpan Elapsed => stopwatch.Elapsed;
    
    public AnalysisProgress(ICreateSnapshotUi createSnapshotUi)
    {
        this.createSnapshotUi = createSnapshotUi ?? throw new ArgumentNullException(nameof(createSnapshotUi));
    }

    public async Task Start(DataSize totalDataSize)
    {
        progress = new DataSizeProgress(totalDataSize);
        lastPercentageAnnounced = 0;
        stopwatch.Start();
        
        await AnnounceProgress();
    }

    public async Task DoProgress(DataSize dataSize)
    {
        progress.Value += dataSize;
        await AnnounceProgress();
    }

    public async Task End()
    {
        stopwatch.Stop();
        
        if (Math.Abs(lastPercentageAnnounced - 100) > float.Epsilon)
            await AnnounceProgress();
    }

    private async Task AnnounceProgress()
    {
        float currentPercentage = progress.Percentage;
        float percentageDifference = currentPercentage - lastPercentageAnnounced;

        if (percentageDifference < 0.1)
            return;

        DiskAnalysisProgressInfo info = new()
        {
            Percentage = progress,
            TotalSize = progress.Size,
            ProcessedSize = progress.Value - progress.MinValue,
            ElapsedTime = stopwatch.Elapsed
        };

        await createSnapshotUi.AnnounceAnalysisProgress(info);

        lastPercentageAnnounced = currentPercentage;
    }
}