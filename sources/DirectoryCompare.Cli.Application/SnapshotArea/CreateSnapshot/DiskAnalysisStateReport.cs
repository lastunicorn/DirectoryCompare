// VeloCity
// Copyright (C) 2022-2023 Dust in the Wind
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

using DustInTheWind.DirectoryCompare.Cli.Application.SnapshotArea.CreateSnapshot.DiskAnalysis;

namespace DustInTheWind.DirectoryCompare.Cli.Application.SnapshotArea.CreateSnapshot;

internal sealed class DiskAnalysisStateReport : IDiskAnalysisStateReport, IDisposable
{
    private readonly ManualResetEventSlim manualResetEventSlim = new(false);

    public event EventHandler<ErrorEncounteredEventArgs> ErrorEncountered;

    public event EventHandler<DiskReaderStartingEventArgs> Starting;

    public event EventHandler<DiskAnalysisProgressEventArgs> Progress;

    public event EventHandler Finished;

    public void WaitToEnd()
    {
        manualResetEventSlim.Wait();
    }

    public void OnErrorEncountered(ErrorEncounteredEventArgs e)
    {
        ErrorEncountered?.Invoke(this, e);
    }

    public void OnStarting(DiskReaderStartingEventArgs e)
    {
        manualResetEventSlim.Reset();
        Starting?.Invoke(this, e);
    }

    public void OnProgress(DiskAnalysisProgressEventArgs e)
    {
        Progress?.Invoke(this, e);
    }

    public void OnFinished()
    {
        manualResetEventSlim.Set();
        Finished?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        manualResetEventSlim?.Dispose();
    }
}