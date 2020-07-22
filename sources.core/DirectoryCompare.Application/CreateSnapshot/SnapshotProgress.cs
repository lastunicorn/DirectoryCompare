using System;
using System.Threading;

namespace DustInTheWind.DirectoryCompare.Application.CreateSnapshot
{
    public class SnapshotProgress
    {
        public event EventHandler<float> ProgressChanged;
        private readonly ManualResetEventSlim manualResetEventSlim = new ManualResetEventSlim(false);

        public void ReportProgress(float value)
        {
            OnProgressChanged(value);
        }

        public void ReportEnd()
        {
            manualResetEventSlim.Set();
        }

        public void WaitToEnd()
        {
            manualResetEventSlim.Wait();
        }

        protected virtual void OnProgressChanged(float e)
        {
            ProgressChanged?.Invoke(this, e);
        }
    }
}