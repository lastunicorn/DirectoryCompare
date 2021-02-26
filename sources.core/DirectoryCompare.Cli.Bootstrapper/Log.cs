using System;
using DustInTheWind.ConsoleFramework.Logging;

namespace DustInTheWind.DirectoryCompare.Cli
{
    internal class Log : ILog
    {
        private readonly Domain.Logging.ILog log;

        public Log(Domain.Logging.ILog log)
        {
            this.log = log;
        }

        public void Write(LogLevel logLevel, string message)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    WriteDebug(message);
                    break;

                case LogLevel.Trace:
                    WriteDebug(message);
                    break;

                case LogLevel.Info:
                    WriteInfo(message);
                    break;

                case LogLevel.Warning:
                    WriteWarning(message);
                    break;

                case LogLevel.Error:
                    WriteError(message);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
        }

        public void Write(LogLevel logLevel, string format, params object[] args)
        {
            string message = string.Format(format, args);
            Write(logLevel, message);
        }

        public void WriteDebug(string message)
        {
            log.WriteDebug(message);
        }

        public void WriteDebug(string format, params object[] args)
        {
            log.WriteDebug(format, args);
        }

        public void WriteInfo(string message)
        {
            log.WriteInfo(message);
        }

        public void WriteInfo(string format, params object[] args)
        {
            log.WriteInfo(format, args);
        }

        public void WriteWarning(string message)
        {
            log.WriteWarning(message);
        }

        public void WriteWarning(string format, params object[] args)
        {
            log.WriteWarning(format, args);
        }

        public void WriteWarning(string message, Exception ex)
        {
            log.WriteWarning(message, ex);
        }

        public void WriteWarning(Exception ex)
        {
            log.WriteWarning(ex);
        }

        public void WriteError(string message)
        {
            log.WriteError(message);
        }

        public void WriteError(string format, params object[] args)
        {
            log.WriteError(format, args);
        }

        public void WriteError(string message, Exception ex)
        {
            log.WriteError(message, ex);
        }

        public void WriteError(Exception ex)
        {
            log.WriteError(ex);
        }
    }
}