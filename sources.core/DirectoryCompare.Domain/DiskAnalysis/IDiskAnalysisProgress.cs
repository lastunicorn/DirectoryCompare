using System;

namespace DustInTheWind.DirectoryCompare.Domain.DiskAnalysis
{
    public interface IDiskAnalysisProgress
    {
        event EventHandler<ErrorEncounteredEventArgs> ErrorEncountered;
        event EventHandler<DiskReaderStartingEventArgs> Starting;
        event EventHandler<DiskAnalysisProgressEventArgs> Progress;

        void WaitToEnd();
    }
}