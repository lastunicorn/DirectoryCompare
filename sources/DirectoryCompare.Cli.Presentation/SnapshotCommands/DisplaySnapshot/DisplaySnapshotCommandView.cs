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

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.SnapshotCommands.DisplaySnapshot;

public class DisplaySnapshotCommandView : ViewBase<DisplaySnapshotCommand>
{
    public override void Display(DisplaySnapshotCommand command)
    {
        if (command.Response == null)
            CustomConsole.WriteLine("There is no snapshot.");
        else
        {
            WriteValue("Pot", command.Response.PotName);
            WriteValue("Snapshot", command.Response.SnapshotId.ToString("D"));
            WriteValue("Path", command.Response.OriginalPath);
            WriteValue("Creation Time", command.Response.SnapshotCreationTime.ToLocalTime());

            Console.WriteLine();

            DirectoryView directoryView = new(command.Response.RootDirectory);
            directoryView.Display();
        }
    }
}