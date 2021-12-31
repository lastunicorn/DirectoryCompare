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
using DustInTheWind.DirectoryCompare.Domain.Comparison;

namespace DustInTheWind.DirectoryCompare.Application.MiscellaneousArea.CompareSnapshots
{
    internal class FileComparisonExporter
    {
        public string ExportName { get; set; }

        public bool AddTimeStamp { get; set; }

        public string ExportDirectoryPath { get; private set; }

        public void Export(SnapshotComparer comparer)
        {
            if (string.IsNullOrWhiteSpace(ExportName))
                throw new Exception("Cannot export comparison result. No export name was provided.");

            ExportDirectoryPath = CreateExportDirectory();

            ExportInfoFile(comparer, ExportDirectoryPath);
            ExportOnlyInSnapshot1(comparer, ExportDirectoryPath);
            ExportOnlyInSnapshot2(comparer, ExportDirectoryPath);
            ExportContentDifferentName(comparer, ExportDirectoryPath);
            ExportSameNameDifferentContent(comparer, ExportDirectoryPath);
        }

        private string CreateExportDirectory()
        {
            string exportDirectoryPathBase = AddTimeStamp
                ? string.Format("{0} - {1:yyyy MM dd HHmmss}", ExportName, DateTime.Now)
                : ExportName;

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

        private static void ExportInfoFile(SnapshotComparer comparer, string exportDirectoryPath)
        {
            string filePath = Path.Combine(exportDirectoryPath, "info.txt");
            using StreamWriter streamWriter = new(filePath);

            WriteFileHeader(streamWriter, comparer);

            streamWriter.WriteLine("StartTime (UTC) : {0}", comparer.StartTimeUtc);
            streamWriter.WriteLine("EndTime (UTC)   : {0}", comparer.EndTimeUtc);
            streamWriter.WriteLine("TotalTime       : {0}", comparer.TotalTime);
        }

        private static void ExportOnlyInSnapshot1(SnapshotComparer comparer, string exportDirectoryPath)
        {
            string filePath = Path.Combine(exportDirectoryPath, "only-in-snapshot1.txt");
            using StreamWriter streamWriter = new(filePath);

            WriteFileHeader(streamWriter, comparer);

            streamWriter.WriteLine("Files only in snapshot 1:");
            foreach (string path in comparer.OnlyInSnapshot1)
                streamWriter.WriteLine(path);
        }

        private static void ExportOnlyInSnapshot2(SnapshotComparer comparer, string exportDirectoryPath)
        {
            string filePath = Path.Combine(exportDirectoryPath, "only-in-snapshot2.txt");
            using StreamWriter streamWriter = new(filePath);

            WriteFileHeader(streamWriter, comparer);

            streamWriter.WriteLine("Files only in snapshot 2:");
            foreach (string path in comparer.OnlyInSnapshot2)
                streamWriter.WriteLine(path);

            streamWriter.WriteLine();
        }

        private static void ExportContentDifferentName(SnapshotComparer comparer, string exportDirectoryPath)
        {
            string filePath = Path.Combine(exportDirectoryPath, "same-content-different-name.txt");
            using StreamWriter streamWriter = new(filePath);

            WriteFileHeader(streamWriter, comparer);

            streamWriter.WriteLine("Different names:");
            foreach (ItemComparison itemComparison in comparer.DifferentNames)
            {
                streamWriter.WriteLine("1 - " + itemComparison.FullName1);
                streamWriter.WriteLine("2 - " + itemComparison.FullName2);
            }
        }

        private static void ExportSameNameDifferentContent(SnapshotComparer comparer, string exportDirectoryPath)
        {
            string filePath = Path.Combine(exportDirectoryPath, "same-name-different-content.txt");
            using StreamWriter streamWriter = new(filePath);

            WriteFileHeader(streamWriter, comparer);

            streamWriter.WriteLine("Different content:");
            foreach (ItemComparison itemComparison in comparer.DifferentContent)
                streamWriter.WriteLine(itemComparison.FullName1);
        }

        private static void WriteFileHeader(TextWriter streamWriter, SnapshotComparer comparer)
        {
            string snapshot1OriginalPath = comparer.Snapshot1.OriginalPath;
            DateTime snapshot1CreationTime = comparer.Snapshot1.CreationTime;
            streamWriter.WriteLine("Snapshot 1: {0} [{1:yyyy MM dd HHmmss}]", snapshot1OriginalPath, snapshot1CreationTime);

            string snapshot2OriginalPath = comparer.Snapshot2.OriginalPath;
            DateTime snapshot2CreationTime = comparer.Snapshot2.CreationTime;
            streamWriter.WriteLine("Snapshot 2: {0} [{1:yyyy MM dd HHmmss}]", snapshot2OriginalPath, snapshot2CreationTime);

            streamWriter.WriteLine();
        }
    }
}