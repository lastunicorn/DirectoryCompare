using System;

namespace DustInTheWind.DirectoryCompare.Domain.Logging
{
    public interface ILog
    {
        void WriteDebug(string message);

        void WriteDebug(string format, params object[] args);

        void WriteInfo(string message);

        void WriteInfo(string format, params object[] args);

        void WriteWarning(string message);

        void WriteWarning(string format, params object[] args);

        void WriteWarning(string message, Exception ex);

        void WriteWarning(Exception ex);

        void WriteError(string message);

        void WriteError(string format, params object[] args);

        void WriteError(string message, Exception ex);

        void WriteError(Exception ex);
    }
}