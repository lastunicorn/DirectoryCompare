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

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.SnapshotCommands.DisplaySnapshot;

public class DisplaySnapshotCommandView : ViewBase<SnapshotViewModel>
{
    public override void Display(SnapshotViewModel snapshotViewModel)
    {
        WriteValue("Pot", snapshotViewModel.PotName);
        WriteValue("Snapshot", snapshotViewModel.SnapshotId.ToString("D"));
        WriteValue("Path", snapshotViewModel.OriginalPath);
        WriteValue("Creation Time", snapshotViewModel.CreationTime.ToLocalTime());
        WriteValue("Directories", snapshotViewModel.TotalDirectoryCount.ToString("N0"));
        WriteValue("Files", snapshotViewModel.TotalFileCount.ToString("N0"));

        string dataSize = snapshotViewModel.DataSize.ToNiceString();
        WriteValue("Data Size", dataSize);

        string storageSize = snapshotViewModel.StorageSize.ToNiceString();
        WriteValue("Snapshot Size", storageSize);

        if (snapshotViewModel.RootDirectory != null)
        {
            Console.WriteLine();

            DirectoryView directoryView = new(snapshotViewModel.RootDirectory);
            directoryView.Display();
        }
    }
}