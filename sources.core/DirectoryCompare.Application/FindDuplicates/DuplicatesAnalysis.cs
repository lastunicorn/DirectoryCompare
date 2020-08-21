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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DustInTheWind.DirectoryCompare.Domain;
using DustInTheWind.DirectoryCompare.Domain.Comparison;

namespace DustInTheWind.DirectoryCompare.Application.FindDuplicates
{
    public class DuplicatesAnalysis
    {
        private readonly ManualResetEventSlim manualResetEvent = new ManualResetEventSlim(false);
        private readonly FileDuplicates fileDuplicates;

        public event EventHandler DuplicateFound;
        public event EventHandler AnalysisFinished;

        private readonly ConcurrentQueue<FileDuplicate> duplicates = new ConcurrentQueue<FileDuplicate>();

        public DuplicatesAnalysisSummary Summary { get; private set; }

        public AnalysisState State { get; private set; }

        public DuplicatesAnalysis(FileDuplicates fileDuplicates)
        {
            this.fileDuplicates = fileDuplicates ?? throw new ArgumentNullException(nameof(fileDuplicates));
        }

        internal Task RunAsync()
        {
            manualResetEvent.Reset();
            State = AnalysisState.Running;

            return Task.Run(() =>
            {
                try
                {
                    int duplicateCount = 0;
                    DataSize totalSize = 0;

                    foreach (FileDuplicate duplicate in fileDuplicates)
                    {
                        duplicateCount++;
                        totalSize += duplicate.Size;
                        duplicates.Enqueue(duplicate);

                        OnDuplicateFound();
                    }

                    Summary = new DuplicatesAnalysisSummary(duplicateCount, totalSize);
                    OnAnalysisFinished();
                }
                finally
                {
                    State = AnalysisState.Finished;
                    manualResetEvent.Set();
                }
            });
        }

        public void WaitToEnd()
        {
            manualResetEvent.Wait();
        }

        protected virtual void OnDuplicateFound()
        {
            DuplicateFound?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnAnalysisFinished()
        {
            AnalysisFinished?.Invoke(this, EventArgs.Empty);
        }

        public IEnumerable<FileDuplicate> GetDuplicatesFromBuffer()
        {
            while (duplicates.TryDequeue(out FileDuplicate localValue))
                yield return localValue;
        }
    }
}