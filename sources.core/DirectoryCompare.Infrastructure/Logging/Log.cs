using System;
using log4net;

namespace DustInTheWind.DirectoryCompare.Infrastructure.Logging
{
    public sealed class Log : Domain.Logging.ILog
    {
        private ILog log;

        private void Open()
        {
            log = LogManager.GetLogger(typeof(Log));
        }

        public void WriteDebug(string message)
        {
            if (log == null)
                Open();

            log.Debug(message);
        }

        public void WriteDebug(string format, params object[] args)
        {
            if (log == null)
                Open();

            log.DebugFormat(format, args);
        }

        public void WriteInfo(string message)
        {
            if (log == null)
                Open();

            log.Info(message);
        }

        public void WriteInfo(string format, params object[] args)
        {
            if (log == null)
                Open();

            log.InfoFormat(format, args);
        }

        public void WriteWarning(string message)
        {
            if (log == null)
                Open();

            log.Warn(message);
        }

        public void WriteWarning(string format, params object[] args)
        {
            if (log == null)
                Open();

            log.WarnFormat(format, args);
        }

        public void WriteWarning(string message, Exception ex)
        {
            if (log == null)
                Open();

            log.Warn(message, ex);
        }

        public void WriteWarning(Exception ex)
        {
            if (log == null)
                Open();

            log.Warn(ex);
        }

        public void WriteError(string message)
        {
            if (log == null)
                Open();

            log.Error(message);
        }

        public void WriteError(string format, params object[] args)
        {
            if (log == null)
                Open();

            log.ErrorFormat(format, args);
        }

        public void WriteError(string message, Exception ex)
        {
            if (log == null)
                Open();

            log.Error(message, ex);
        }

        public void WriteError(Exception ex)
        {
            if (log == null)
                Open();

            log.Error(ex);
        }
    }
}