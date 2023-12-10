// DirectoryCompare
// Copyright (C) 2017-2023 Dust in the Wind
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
using DustInTheWind.DirectoryCompare.DataStructures;
using DustInTheWind.DirectoryCompare.Ports.UserAccess;

namespace DustInTheWind.DirectoryCompare.UserAccess;

public class CreateSnapshotUi : EnhancedConsole, ICreateSnapshotUi
{
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
        CustomConsole.WriteLine($"Indexed so far: {fileIndexInfo.FileCount:N0} files ({fileIndexInfo.DataSize})");

        return Task.CompletedTask;
    }

    public Task AnnounceFilesIndexed(FileIndexInfo fileIndexInfo)
    {
        CustomConsole.WriteLine();

        WithIndentation(() =>
        {
            CustomConsole.WriteLineSuccess($"File count: {fileIndexInfo.FileCount:N0}");
            CustomConsole.WriteLineSuccess($"Data Size: {fileIndexInfo.DataSize.ToString("D")}");
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
        Console.WriteLine($"Progress: {info.Percentage:0.00} % ({info.ProcessedSize} / {info.TotalSize})");

        return Task.CompletedTask;
    }

    public Task AnnounceAnalysisFinished()
    {
        WriteSuccess("Done");
        
        return Task.CompletedTask;
    }
}