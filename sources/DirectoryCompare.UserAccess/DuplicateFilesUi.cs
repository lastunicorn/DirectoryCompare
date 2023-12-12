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

public class DuplicateFilesUi : EnhancedConsole, IDuplicateFilesUi
{
    public Task AnnounceStart(DuplicateSearchStartedInfo info)
    {
        CustomConsole.WriteLine("Searching for duplicates between:");

        WithIndentation(() =>
        {
            WriteValue("Snapshot 1", info.SnapshotLeft);
            WriteValue("Snapshot 2", info.SnapshotRight);
        });

        CustomConsole.WriteLine();

        return Task.CompletedTask;
    }

    public Task AnnounceDuplicate(DuplicateFoundInfo filePair)
    {
        Console.WriteLine(filePair.FullPathLeft);
        Console.WriteLine(filePair.FullPathRight);
        
        DataSize sizeShort = filePair.Size;
        string sizeLong = filePair.Size.ToString(DataSizeUnit.Byte);
        FileHash fileHash = filePair.Hash;
        CustomConsole.WriteLine(ConsoleColor.DarkGray, $"{sizeShort} ({sizeLong}) - {fileHash}");
        
        Console.WriteLine();

        return Task.CompletedTask;
    }

    public Task AnnounceFinished(DuplicateSearchFinishedInfo info)
    {
        WriteValue("Duplicates", info.DuplicateCount.ToString("N0"));
        WriteValue("Total size", info.TotalSize.ToString("D"));
        WriteValue("Elapsed Time", info.ElapsedTime);
        Console.WriteLine();

        return Task.CompletedTask;
    }
}