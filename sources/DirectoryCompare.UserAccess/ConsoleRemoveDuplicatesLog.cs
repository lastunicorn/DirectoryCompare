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

using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.DirectoryCompare.DataStructures;
using DustInTheWind.DirectoryCompare.Ports.UserAccess;

namespace DustInTheWind.DirectoryCompare.UserAccess;

public class ConsoleRemoveDuplicatesLog : EnhancedConsole, IRemoveDuplicatesLog
{
    public void WritePlanInfo(RemoveDuplicatesPlan removeDuplicatesPlan)
    {
        WithIndentation("Removing duplicates:", () =>
        {
            WriteValue("Snapshot Left", removeDuplicatesPlan.SnapshotLeft);
            WriteValue("Snapshot Right:", removeDuplicatesPlan.SnapshotRight);
            WriteValue("Remove Part:", removeDuplicatesPlan.RemovePart);

            string action = removeDuplicatesPlan.PurgatoryDirectory == null
                ? "delete"
                : "move";
            WriteValue("Action", action);

            if (removeDuplicatesPlan.PurgatoryDirectory != null)
                WriteValue("Move to directory", removeDuplicatesPlan.PurgatoryDirectory);
        });

        Console.WriteLine();
    }

    public void DuplicateFound(string fullPathLeft, string fullPathRight)
    {
        WithIndentation("Duplicate found:", () =>
        {
            WriteValue("Left ", fullPathLeft);
            WriteValue("Right", fullPathRight);
        });
    }

    public void WriteActionNoFileExists()
    {
        WithIndentation(() =>
        {
            WriteInfo("Action: [none]; None of the files exists on disk.");
        });

        Console.WriteLine();
    }

    public void WriteActionFileToKeepDoesNotExist(string path)
    {
        WithIndentation(() =>
        {
            WriteInfo($"Action: [none]; File to keep does not exist on disk. File: {path}");
        });

        Console.WriteLine();
    }

    public void WriteActionFileIsAlreadyRemoved(string path)
    {
        WithIndentation(() =>
        {
            WriteInfo($"Action: [none]; File scheduled to be removed does not exist. File: {path}");
        });

        Console.WriteLine();
    }

    public void WriteActionFileDeleted(string path)
    {
        WithIndentation(() =>
        {
            WriteInfo($"Action: [deleted]; File: {path}");
        });

        Console.WriteLine();
    }

    public void WriteActionFileMoved(string path)
    {
        WithIndentation(() =>
        {
            WriteInfo($"Action: [moved]; File: {path}");
        });

        Console.WriteLine();
    }

    public void WriteSummary(int removedFiles, DataSize removedSize)
    {
        Console.WriteLine("Total files removed: " + removedFiles);
        Console.WriteLine($"Total size: {removedSize} ({removedSize.ToString(DataSizeUnit.Byte)})");
        Console.WriteLine();
    }
}