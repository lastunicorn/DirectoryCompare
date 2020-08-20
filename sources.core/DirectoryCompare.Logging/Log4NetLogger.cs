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

using DustInTheWind.DirectoryCompare.Domain.Logging;
using log4net;

namespace DustInTheWind.DirectoryCompare.Logging
{
    public sealed class Log4NetLogger : IProjectLogger
    {
        private ILog log;

        public void Debug(string text)
        {
            if (log == null)
                Open();

            log.Debug(text);
        }

        public void Debug(string format, params object[] arg)
        {
            if (log == null)
                Open();

            string text = arg == null ? format : string.Format(format, arg);
            log.Debug(text);
        }

        public void Info(string text)
        {
            if (log == null)
                Open();

            log.Info(text);
        }

        public void Info(string format, params object[] arg)
        {
            if (log == null)
                Open();

            string text = arg == null ? format : string.Format(format, arg);
            log.Info(text);
        }

        public void Warn(string text)
        {
            if (log == null)
                Open();

            log.Warn(text);
        }

        public void Warn(string format, params object[] arg)
        {
            if (log == null)
                Open();

            string text = arg == null ? format : string.Format(format, arg);
            log.Warn(text);
        }

        public void Error(string text)
        {
            if (log == null)
                Open();

            log.Error(text);
        }

        public void Error(string format, params object[] arg)
        {
            if (log == null)
                Open();

            string text = arg == null ? format : string.Format(format, arg);
            log.Error(text);
        }

        private void Open()
        {
            log = LogManager.GetLogger(typeof(ProjectLogger));
        }
    }
}