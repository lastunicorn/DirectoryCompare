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
using DustInTheWind.ConsoleFramework.Logging;

namespace DustInTheWind.DirectoryCompare.Cli.Bootstrapper
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