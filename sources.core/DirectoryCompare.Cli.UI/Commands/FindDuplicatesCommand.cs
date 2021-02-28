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
using System.Collections.Generic;
using System.Linq;
using DustInTheWind.ConsoleFramework;
using DustInTheWind.DirectoryCompare.Application.MiscelaneousArea.FindDuplicates;
using DustInTheWind.DirectoryCompare.Cli.UI.Views;
using DustInTheWind.DirectoryCompare.Domain.Comparison;
using DustInTheWind.DirectoryCompare.Infrastructure;

namespace DustInTheWind.DirectoryCompare.Cli.UI.Commands
{
    public class FindDuplicatesCommand : ICommand
    {
        private readonly RequestBus requestBus;
        private readonly FindDuplicatesView findDuplicatesView;

        public string Key { get; } = "find-duplicates";

        public string Description => string.Empty;

        public FindDuplicatesCommand(RequestBus requestBus)
        {
            this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));

            findDuplicatesView = new FindDuplicatesView();
        }

        public void Execute(Arguments arguments)
        {
            FindDuplicatesRequest request = CreateRequest(arguments);
            DuplicatesAnalysis analysis = requestBus.PlaceRequest<FindDuplicatesRequest, DuplicatesAnalysis>(request).Result;
            analysis.DuplicateFound += HandleDuplicateFound;

            DisplayDuplicatesFromBuffer(analysis);

            analysis.WaitToEnd();

            DisplaySummary(analysis);
        }

        private void HandleDuplicateFound(object sender, EventArgs e)
        {
            if (sender is DuplicatesAnalysis analysis)
                DisplayDuplicatesFromBuffer(analysis);
        }

        private void DisplayDuplicatesFromBuffer(DuplicatesAnalysis analysis)
        {
            IEnumerable<FileDuplicate> duplicates = analysis.GetDuplicatesFromBuffer();

            foreach (FileDuplicate fileDuplicate in duplicates)
                findDuplicatesView.WriteDuplicate(fileDuplicate);
        }

        private void DisplaySummary(DuplicatesAnalysis analysis)
        {
            findDuplicatesView.WriteSummary(analysis.Summary.DuplicateCount, analysis.Summary.TotalSize);
        }

        private static FindDuplicatesRequest CreateRequest(Arguments arguments)
        {
            if (arguments.Count == 0)
                throw new Exception("Invalid command parameters.");

            Argument[] anonymousArguments = arguments.GetAnonymousArguments().ToArray();

            if (anonymousArguments.Length == 0)
                throw new Exception("Invalid command parameters.");

            string left = anonymousArguments.Length >= 1
                ? anonymousArguments[0].Value
                : null;

            string right = anonymousArguments.Length >= 2
                ? anonymousArguments[1].Value
                : null;

            bool checkFilesExist = arguments.GetBoolValue("x");

            return new FindDuplicatesRequest
            {
                SnapshotLeft = left,
                SnapshotRight = right,
                CheckFilesExist = checkFilesExist
            };
        }
    }
}