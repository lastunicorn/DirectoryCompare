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

using DustInTheWind.DirectoryCompare.DataStructures;
using DustInTheWind.DirectoryCompare.Ports.LogAccess;

namespace DustInTheWind.DirectoryCompare.UserAccess;

public class ConsoleRemoveDuplicatesLog : IRemoveDuplicatesLog
{
    public void WritePlanInfo(RemoveDuplicatesPlan removeDuplicatesPlan)
    {
        Console.WriteLine("Removing duplicates");
        Console.WriteLine("  Snapshot Left:  " + removeDuplicatesPlan.SnapshotLeft);
        Console.WriteLine("  Snapshot Right: " + removeDuplicatesPlan.SnapshotRight);
        Console.WriteLine("  Remove Part: " + removeDuplicatesPlan.RemovePart);

        string action = removeDuplicatesPlan.PurgatoryDirectory == null
            ? "delete"
            : "move";
        Console.WriteLine("  Action: " + action);

        if (removeDuplicatesPlan.PurgatoryDirectory != null)
            Console.WriteLine("  Move to directory: " + removeDuplicatesPlan.PurgatoryDirectory);

        Console.WriteLine();
    }

    public void DuplicateFound(string fullPathLeft, string fullPathRight)
    {
        Console.WriteLine("Duplicate found:");
        Console.WriteLine("  Left:  " + fullPathLeft);
        Console.WriteLine("  Right: " + fullPathRight);
    }

    public void WriteActionNoFileExists()
    {
        Console.WriteLine("  Action: [none] None of the files exists on disk.");
        Console.WriteLine();
    }

    public void WriteActionFileToKeepDoesNotExist()
    {
        Console.WriteLine("  Action: [none] Only the file scheduled to be removed exists on disk.");
        Console.WriteLine();
    }

    public void WriteActionFileIsAlreadyRemoved()
    {
        Console.WriteLine("  Action: [none] File scheduled to be removed does not exist.");
        Console.WriteLine();
    }

    public void WriteActionFileDeleted(string path)
    {
        Console.WriteLine($"  Action: [deleted] File: {path}");
        Console.WriteLine();
    }

    public void WriteActionFileMoved(string path)
    {
        Console.WriteLine($"  Action: [moved] File: {path}");
        Console.WriteLine();
    }

    public void WriteSummary(int removedFiles, DataSize removedSize)
    {
        Console.WriteLine("Total files removed: " + removedFiles);
        Console.WriteLine($"Total size: {removedSize} ({removedSize.ToString(DataSizeUnit.Byte)})");
        Console.WriteLine();
    }
}