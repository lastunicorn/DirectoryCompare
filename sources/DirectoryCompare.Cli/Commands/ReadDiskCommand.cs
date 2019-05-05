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
using System.Diagnostics;
using System.IO;
using System.Linq;
using DirectoryCompare.CliFramework;
using DustInTheWind.DirectoryCompare.JsonExport;
using DustInTheWind.DirectoryCompare.Serialization;
using DustInTheWind.DirectoryCompare.Utils;

namespace DustInTheWind.DirectoryCompare.Cli.Commands
{
    internal class ReadDiskCommand : ICommand
    {
        private readonly Stopwatch stopwatch;
        private DiskReader diskReader;

        public ProjectLogger Logger { get; set; }
        public string SourcePath { get; set; }
        public string DestinationFilePath { get; set; }

        public PathCollection BlackList { get; set; } = new PathCollection();

        public ReadDiskCommand()
        {
            stopwatch = new Stopwatch();
        }

        public void DisplayInfo()
        {
        }

        public void Initialize(Arguments arguments)
        {
            Logger = new ProjectLogger();
            SourcePath = arguments[0];
            DestinationFilePath = arguments[1];
            BlackList = ReadBlackList(arguments[2]);
        }

        private static PathCollection ReadBlackList(string filePath)
        {
            Console.WriteLine("Reading black list from file: {0}", filePath);

            string[] list = File.Exists(filePath)
                ? File.ReadAllLines(filePath)
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Where(x => !x.StartsWith("#"))
                    .ToArray()
                : new string[0];

            return new PathCollection(list);
        }

        public void Execute()
        {
            Logger?.Open();

            try
            {
                if (SourcePath == null)
                    throw new Exception("SourcePath was not provided.");

                if (!Directory.Exists(SourcePath))
                    throw new Exception("The SourcePath does not exist.");

                JsonDiskExport jsonDiskExport = new JsonDiskExport(new StreamWriter(DestinationFilePath));
                diskReader = new DiskReader(SourcePath, jsonDiskExport);
                diskReader.Starting += HandleDiskReaderStarting;
                diskReader.BlackList.AddRange(BlackList);
                diskReader.ErrorEncountered += HandleDiskReaderErrorEncountered;

                ScanPath();
                WriteToFile();
            }
            finally
            {
                Logger?.Close();
            }
        }

        private void HandleDiskReaderStarting(object sender, DiskReaderStartingEventArgs e)
        {
            Console.WriteLine("Computed black list:");

            foreach (string blackListItem in e.BlackList)
                Console.WriteLine("- " + blackListItem);
        }

        private void HandleDiskReaderErrorEncountered(object sender, ErrorEncounteredEventArgs e)
        {
            Logger?.Error("Error while reading path '{0}': {1}", e.Path, e.Exception);
        }

        private void ScanPath()
        {
            Logger?.Info("Scanning path: {0}", SourcePath);
            Logger?.Info("Results file: {0}", DestinationFilePath);
            Logger?.Info("Black List:");

            if (BlackList != null)
                foreach (string blackListItem in BlackList)
                    Logger?.Info(blackListItem);

            stopwatch.Reset();
            stopwatch.Start();
            diskReader.Read();
            stopwatch.Stop();

            Logger?.Info("Finished scanning path {0}", stopwatch.Elapsed);
        }

        private void WriteToFile()
        {
            //JsonFileSerializer serializer = new JsonFileSerializer();
            //serializer.WriteToFile(containerDiskExport.Container, DestinationFilePath);

            Logger?.Info("Finished writing container into file: {0}", DestinationFilePath);
        }
    }
}