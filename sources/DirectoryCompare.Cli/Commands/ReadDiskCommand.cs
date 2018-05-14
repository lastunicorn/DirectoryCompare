// DirectoryCompare
// Copyright (C) 2017 Dust in the Wind
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
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace DustInTheWind.DirectoryCompare.Cli.Commands
{
    internal class ReadDiskCommand : ICommand
    {
        private readonly Stopwatch stopwatch;
        private IContainerProvider diskReader;

        public ProjectLogger Logger { get; set; }
        public string SourcePath { get; set; }
        public string DestinationFilePath { get; set; }

        public ReadDiskCommand()
        {
            stopwatch = new Stopwatch();
        }

        public void Execute()
        {
            Logger?.Open();

            if (SourcePath == null)
                throw new Exception("SourcePath was not provided.");

            if (!Directory.Exists(SourcePath))
                throw new Exception("The SourcePath does not exist.");

            diskReader = new DiskReaderAsync(SourcePath);

            ScanPath();
            WriteToFile();
        }

        private void ScanPath()
        {
            Logger?.Info("Scanning path: {0}", SourcePath);

            stopwatch.Reset();
            stopwatch.Start();
            diskReader.Read();
            stopwatch.Stop();

            Logger?.Info("Finished scanning path {0}", stopwatch.Elapsed);
        }

        private void WriteToFile()
        {
            stopwatch.Start();

            string json = JsonConvert.SerializeObject(diskReader.Container);
            File.WriteAllText(DestinationFilePath, json);

            stopwatch.Stop();

            Logger?.Info("Finished writing container into file: {0}", DestinationFilePath);
        }
    }
}