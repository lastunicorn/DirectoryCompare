// DirectoryCompare
// Copyright (C) 2017-2024 Dust in the Wind
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

using DustInTheWind.ConsoleTools;
using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.DirectoryCompare.Cli.Presentation.Utils;
using DustInTheWind.DirectoryCompare.Ports.UserAccess;

namespace DustInTheWind.DirectoryCompare.UserAccess;

public class CreateSnapshotUi : EnhancedConsole, ICreateSnapshotUi
{
    public DataSizeFormat DataSizeFormat { get; set; }

    public Task AnnounceStarting(StartNewSnapshotInfo info)
    {
        WithIndentation("Creating a new snapshot", () =>
        {
            WriteValue("Pot Name", info.PotName);
            WriteValue("Path", info.Path);

            if (info.BlackList is { Count: > 0 })
            {
                WriteValue("Black Listed Paths", " ");

                WithIndentation(() =>
                {
                    foreach (string blackPath in info.BlackList)
                        WriteInfo($"- {blackPath}");
                });
            }

            WriteValue("Start Time", info.StartTime.ToLocalTime());
        });

        CustomConsole.WriteLine();

        return Task.CompletedTask;
    }

    public Task AnnounceFilesIndexing()
    {
        CustomConsole.WriteLine("Counting files...");

        return Task.CompletedTask;
    }

    public Task AnnounceFileIndexingProgress(FileIndexInfo fileIndexInfo)
    {
        CustomConsole.Write("Indexed:");
        
        int fileCount = fileIndexInfo.FileCount;
        DataSizeDisplay dataSizeDisplay = fileIndexInfo.DataSize.ToDataSizeDisplay(DataSizeFormat);
        CustomConsole.WriteLine(ConsoleColor.DarkGray, $" {fileCount:N0} files ({dataSizeDisplay})");

        return Task.CompletedTask;
    }

    public Task AnnounceFilesIndexed(FileIndexInfo fileIndexInfo)
    {
        CustomConsole.WriteLine();

        WithIndentation(() =>
        {
            CustomConsole.WriteLineSuccess($"File count: {fileIndexInfo.FileCount:N0}");
            CustomConsole.WriteLineSuccess($"Data Size: {fileIndexInfo.DataSize.ToDataSizeDisplay(DataSizeFormat | DataSizeFormat.Detailed)}");
        });

        CustomConsole.WriteLine();

        return Task.CompletedTask;
    }

    public Task AnnounceFileIndexingError(IndexingErrorInfo info)
    {
        CustomConsole.WriteLineError($"Error while indexing path: {info.Path}");
        CustomConsole.WriteLineError(info.Exception);

        return Task.CompletedTask;
    }

    public Task AnnounceAnalysisError(AnalysisErrorInfo info)
    {
        CustomConsole.WriteLineError($"Error while indexing path: {info.Path}");
        CustomConsole.WriteLineError(info.Exception);

        return Task.CompletedTask;
    }

    public Task AnnounceAnalysisProgress(DiskAnalysisProgressInfo info)
    {
        CustomConsole.Write("Progress:");
        CustomConsole.WriteEmphasized($" {info.Percentage:0.00} %");
        CustomConsole.WriteLine(ConsoleColor.DarkGray, $" ({info.ProcessedSize} / {info.TotalSize.ToDataSizeDisplay(DataSizeFormat)})");

        return Task.CompletedTask;
    }

    public Task AnnounceAnalysisFinished(AnalysisFinishedInfo info)
    {
        CustomConsole.WriteLine();

        WriteSuccess("Done");
        WriteSuccess($"Total time: {info.ElapsedTime}");

        return Task.CompletedTask;
    }

    public Task AnnounceAnalysisStarting()
    {
        CustomConsole.WriteLine("Analysing files...");

        return Task.CompletedTask;
    }
}