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
    public Task AnnounceStart(SnapshotLocation snapshotLeft, SnapshotLocation snapshotRight)
    {
        CustomConsole.WriteLine("Searching for duplicates between:");

        WithIndentation(() =>
        {
            WriteValue("Snapshot 1", snapshotLeft);
            WriteValue("Snapshot 2", snapshotRight);
        });

        CustomConsole.WriteLine();

        return Task.CompletedTask;
    }

    public Task AnnounceDuplicate(FilePairDto filePair)
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

    public Task AnnounceFinished(int duplicateCount, DataSize totalSize)
    {
        WriteValue("Duplicates", duplicateCount.ToString("N0"));
        WriteValue("Total size", totalSize.ToString("D"));
        Console.WriteLine();

        return Task.CompletedTask;
    }
}