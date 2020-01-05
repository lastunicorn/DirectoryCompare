// DirectoryCompare
// Copyright (C) 2017-2019 Dust in the Wind
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
using DustInTheWind.DirectoryCompare.Domain.Logging;
using log4net;

namespace DustInTheWind.DirectoryCompare.Logging
{
    public sealed class Log4NetLogger : IProjectLogger
    {
        private ILog log;

        public Log4NetLogger()
        {
        }

        public void Open()
        {
            log = LogManager.GetLogger(typeof(ProjectLogger));
        }

        public void Close()
        {
        }

        public void Debug(string text)
        {
            log.Debug(text);
        }

        public void Debug(string format, params object[] arg)
        {
            string text = arg == null ? format : string.Format(format, arg);
            log.Debug(text);
        }

        public void Info(string text)
        {
            log.Info(text);
        }

        public void Info(string format, params object[] arg)
        {
            string text = arg == null ? format : string.Format(format, arg);
            log.Info(text);
        }

        public void Warn(string text)
        {
            log.Warn(text);
        }

        public void Warn(string format, params object[] arg)
        {
            string text = arg == null ? format : string.Format(format, arg);
            log.Warn(text);
        }

        public void Error(string text)
        {
            log.Error(text);
        }

        public void Error(string format, params object[] arg)
        {
            string text = arg == null ? format : string.Format(format, arg);
            log.Error(text);
        }
    }
}