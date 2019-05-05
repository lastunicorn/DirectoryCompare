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
using System.IO;
using DustInTheWind.ConsoleTools;
using DustInTheWind.DirectoryCompare.Cli.Commands;

namespace DustInTheWind.DirectoryCompare.Cli.ResultExporters
{
    internal class FileComparisonExporter : IComparisonExporter
    {
        public string ResultsDirectory { get; set; }

        public void Export(ContainerComparer comparer)
        {
            if (string.IsNullOrWhiteSpace(ResultsDirectory))
                throw new Exception("Cannot export comparation result. No file name was provided.");

            string exportDirectoryPath = CreateExportDirectory();

            ExportInfoFile(comparer, exportDirectoryPath);
            ExportOnlyInContainer1(comparer, exportDirectoryPath);
            ExportOnlyInContainer2(comparer, exportDirectoryPath);
            ExportContentDifferentName(comparer, exportDirectoryPath);
            ExportSameNameDifferentContent(comparer, exportDirectoryPath);

            CustomConsole.WriteLine("Results exported into directory: {0}", exportDirectoryPath);
        }

        private string CreateExportDirectory()
        {
            string exportDirectoryPathBase = string.Format("{0} - {1:yyyy MM dd HHmmss}", ResultsDirectory, DateTime.Now);

            string exportDirectoryPath = exportDirectoryPathBase;
            int index = 0;

            while (Directory.Exists(exportDirectoryPath))
            {
                index++;
                exportDirectoryPath = $"{exportDirectoryPathBase} - {index}";
            }

            Directory.CreateDirectory(exportDirectoryPath);

            return exportDirectoryPath;
        }

        private static void ExportInfoFile(ContainerComparer comparer, string exportDirectoryPath)
        {
            string filePath = Path.Combine(exportDirectoryPath, "info.txt");

            using (StreamWriter streamWriter = new StreamWriter(filePath))
            {
                streamWriter.WriteLine("Container 1: {0}", comparer.Container1.OriginalPath);
                streamWriter.WriteLine("Container 2: {0}", comparer.Container2.OriginalPath);

                streamWriter.WriteLine();

                streamWriter.WriteLine("StartTime (UTC) : {0}", comparer.StartTimeUtc);
                streamWriter.WriteLine("EndTime (UTC)   : {0}", comparer.EndTimeUtc);
                streamWriter.WriteLine("TotalTime       : {0}", comparer.TotalTime);
            }
        }

        private static void ExportOnlyInContainer1(ContainerComparer comparer, string exportDirectoryPath)
        {
            string filePath = Path.Combine(exportDirectoryPath, "only-in-container1.txt");

            using (StreamWriter streamWriter = new StreamWriter(filePath))
            {
                streamWriter.WriteLine("Container 1: {0}", comparer.Container1.OriginalPath);
                streamWriter.WriteLine("Container 2: {0}", comparer.Container2.OriginalPath);

                streamWriter.WriteLine();

                streamWriter.WriteLine("Files only in container 1:");
                foreach (string path in comparer.OnlyInContainer1)
                    streamWriter.WriteLine(path);
            }
        }

        private static void ExportOnlyInContainer2(ContainerComparer comparer, string exportDirectoryPath)
        {
            string filePath = Path.Combine(exportDirectoryPath, "only-in-container2.txt");

            using (StreamWriter streamWriter = new StreamWriter(filePath))
            {
                streamWriter.WriteLine("Container 1: {0}", comparer.Container1.OriginalPath);
                streamWriter.WriteLine("Container 2: {0}", comparer.Container2.OriginalPath);

                streamWriter.WriteLine();

                streamWriter.WriteLine("Files only in container 2:");
                foreach (string path in comparer.OnlyInContainer2)
                    streamWriter.WriteLine(path);

                streamWriter.WriteLine();
            }
        }

        private static void ExportContentDifferentName(ContainerComparer comparer, string exportDirectoryPath)
        {
            string filePath = Path.Combine(exportDirectoryPath, "same-content-different-name.txt");

            using (StreamWriter streamWriter = new StreamWriter(filePath))
            {
                streamWriter.WriteLine("Container 1: {0}", comparer.Container1.OriginalPath);
                streamWriter.WriteLine("Container 2: {0}", comparer.Container2.OriginalPath);

                streamWriter.WriteLine();

                streamWriter.WriteLine("Different names:");
                foreach (ItemComparison itemComparison in comparer.DifferentNames)
                {
                    streamWriter.WriteLine("1 - " + itemComparison.FullName1);
                    streamWriter.WriteLine("2 - " + itemComparison.FullName2);
                }
            }
        }

        private static void ExportSameNameDifferentContent(ContainerComparer comparer, string exportDirectoryPath)
        {
            string filePath = Path.Combine(exportDirectoryPath, "same-name-different-content.txt");

            using (StreamWriter streamWriter = new StreamWriter(filePath))
            {
                streamWriter.WriteLine("Container 1: {0}", comparer.Container1.OriginalPath);
                streamWriter.WriteLine("Container 2: {0}", comparer.Container2.OriginalPath);

                streamWriter.WriteLine();

                streamWriter.WriteLine("Different content:");
                foreach (ItemComparison itemComparison in comparer.DifferentContent)
                    streamWriter.WriteLine(itemComparison.FullName1);
            }
        }
    }
}