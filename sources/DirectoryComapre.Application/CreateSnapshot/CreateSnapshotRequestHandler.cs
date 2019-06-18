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
using DustInTheWind.DirectoryCompare.DiskAnalysis;
using DustInTheWind.DirectoryCompare.JsonHashesFile.JsonExport;
using DustInTheWind.DirectoryCompare.Logging;
using DustInTheWind.DirectoryCompare.Utils;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Application.CreateSnapshot
{
    public class CreateSnapshotRequestHandler : RequestHandler<CreateSnapshotRequest>
    {
        private readonly IProjectLogger logger;
        private readonly IDiskAnalyzerFactory diskAnalyzerFactory;
        private PathCollection blackList = new PathCollection();

        public CreateSnapshotRequestHandler(IProjectLogger logger, IDiskAnalyzerFactory diskAnalyzerFactory)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.diskAnalyzerFactory = diskAnalyzerFactory ?? throw new ArgumentNullException(nameof(diskAnalyzerFactory));
        }

        protected override void Handle(CreateSnapshotRequest request)
        {
            blackList = ReadBlackList(request.BlackListFilePath);

            if (request.SourcePath == null)
                throw new Exception("SourcePath was not provided.");

            if (!Directory.Exists(request.SourcePath))
                throw new Exception("The SourcePath does not exist.");

            logger.Info("Scanning path: {0}", request.SourcePath);
            logger.Info("Results file: {0}", request.DestinationFilePath);
            logger.Info("Black List:");

            if (blackList != null)
                foreach (string blackListItem in blackList)
                    logger.Info(blackListItem);

            using (StreamWriter streamWriter = new StreamWriter(request.DestinationFilePath))
            {
                AnalysisRequest analysisRequest = new AnalysisRequest
                {
                    RootPath = request.SourcePath,
                    BlackList = blackList
                };
                JsonAnalysisExport jsonAnalysisExport = new JsonAnalysisExport(streamWriter);
                IDiskAnalyzer diskAnalyzer = diskAnalyzerFactory.Create(analysisRequest, jsonAnalysisExport);
                diskAnalyzer.Starting += HandleDiskReaderStarting;
                diskAnalyzer.ErrorEncountered += HandleDiskReaderErrorEncountered;

                Stopwatch stopwatch = Stopwatch.StartNew();
                diskAnalyzer.Run();
                stopwatch.Stop();

                logger.Info("Finished scanning path {0}", stopwatch.Elapsed);
            }
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

        private static void HandleDiskReaderStarting(object sender, DiskReaderStartingEventArgs e)
        {
            Console.WriteLine("Computed black list:");

            foreach (string blackListItem in e.BlackList)
                Console.WriteLine("- " + blackListItem);
        }

        private void HandleDiskReaderErrorEncountered(object sender, ErrorEncounteredEventArgs e)
        {
            logger.Error("Error while reading path '{0}': {1}", e.Path, e.Exception);
        }
    }
}