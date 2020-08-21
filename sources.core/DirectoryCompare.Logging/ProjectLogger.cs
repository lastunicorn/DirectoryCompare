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
using System.IO;
using DustInTheWind.ConsoleTools;
using DustInTheWind.DirectoryCompare.Domain.Logging;

namespace DustInTheWind.DirectoryCompare.Logging
{
    public sealed class ProjectLogger : IProjectLogger, IDisposable
    {
        private readonly string basePath;
        private StreamWriter streamWriter;
        private bool isDisposed;

        public ProjectLogger()
        {
            basePath = Environment.CurrentDirectory;
        }

        public void Debug(string format)
        {
            if (isDisposed)
                throw new ObjectDisposedException(nameof(ProjectLogger));

            if (streamWriter == null)
                Open();

            Debug(format, new object[0]);

        }

        public void Debug(string format, params object[] arg)
        {
            if (isDisposed)
                throw new ObjectDisposedException(nameof(ProjectLogger));

            if (streamWriter == null)
                Open();

            string text = arg == null ? format : string.Format(format, arg);
            text = string.Format("[{0:yyyy-MM-dd HH:mm:ss.fff}] DEBUG {1}", DateTime.Now, text);

            streamWriter?.WriteLine(text);

            CustomConsole.WriteLine(text);
        }

        public void Info(string format)
        {
            if (isDisposed)
                throw new ObjectDisposedException(nameof(ProjectLogger));

            if (streamWriter == null)
                Open();

            Info(format, new object[0]);
        }

        public void Info(string format, params object[] arg)
        {
            if (isDisposed)
                throw new ObjectDisposedException(nameof(ProjectLogger));

            if (streamWriter == null)
                Open();

            string text = arg == null ? format : string.Format(format, arg);
            text = string.Format("[{0:yyyy-MM-dd HH:mm:ss.fff}] INFO {1}", DateTime.Now, text);

            streamWriter?.WriteLine(text);

            CustomConsole.WriteLine(text);
        }

        public void Warn(string format)
        {
            if (isDisposed)
                throw new ObjectDisposedException(nameof(ProjectLogger));

            if (streamWriter == null)
                Open();

            Warn(format, new object[0]);
        }

        public void Warn(string format, params object[] arg)
        {
            if (isDisposed)
                throw new ObjectDisposedException(nameof(ProjectLogger));

            if (streamWriter == null)
                Open();

            string text = arg == null ? format : string.Format(format, arg);
            text = string.Format("[{0:yyyy-MM-dd HH:mm:ss.fff}] WARN {1}", DateTime.Now, text);

            streamWriter?.WriteLine(text);

            CustomConsole.WriteLine(text);
        }

        public void Error(string format)
        {
            if (isDisposed)
                throw new ObjectDisposedException(nameof(ProjectLogger));

            if (streamWriter == null)
                Open();

            Error(format, new object[0]);
        }

        public void Error(string format, params object[] arg)
        {
            if (isDisposed)
                throw new ObjectDisposedException(nameof(ProjectLogger));

            if (streamWriter == null)
                Open();

            string text = arg == null ? format : string.Format(format, arg);
            text = string.Format("[{0:yyyy-MM-dd HH:mm:ss.fff}] ERROR {1}", DateTime.Now, text);

            streamWriter?.WriteLine(text);

            CustomConsole.WriteLineError(text);
        }

        private void Open()
        {
            string logFilePath = Path.Combine(basePath, string.Format("{0:yyyy MM dd HHmmss}.log", DateTime.UtcNow));
            streamWriter = new StreamWriter(logFilePath);
        }

        public void Dispose()
        {
            if (isDisposed)
                return;

            streamWriter?.Flush();
            streamWriter?.Close();
            streamWriter?.Dispose();
            streamWriter = null;

            isDisposed = true;
        }
    }
}