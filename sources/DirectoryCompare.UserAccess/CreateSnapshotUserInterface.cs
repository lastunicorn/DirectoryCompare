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
using DustInTheWind.DirectoryCompare.Ports.UserAccess;

namespace DustInTheWind.DirectoryCompare.UserAccess;

public class CreateSnapshotUserInterface : ICreateSnapshotUserInterface
{
    public Task AnnounceSnapshotCreating(StartNewSnapshotInfo info)
    {
        CustomConsole.WriteLine("Creating a new snapshot:");
        WriteLabeledValue("Pot Name", info.PotName);
        WriteLabeledValue("Path", info.Path);

        if (info.BlackList is { Count: > 0 })
        {
            CustomConsole.Write("  ");
            CustomConsole.WriteEmphasized("Black Listed Paths:");

            foreach (string blackPath in info.BlackList)
                CustomConsole.WriteLine("    - " + blackPath);
        }
        
        WriteLabeledValue("Start Time", info.StartTime);
        CustomConsole.WriteLine();

        return Task.CompletedTask;
    }

    private static void WriteLabeledValue(string label, object value)
    {
        CustomConsole.Write("  ");
        CustomConsole.WriteEmphasized(label + ": ");
        CustomConsole.WriteLine(ConsoleColor.DarkGray, value);
    }
}