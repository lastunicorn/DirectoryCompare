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
using DustInTheWind.DirectoryCompare.Cli.Application.MiscellaneousArea.PresentDuplicates;
using DustInTheWind.DirectoryCompare.DataStructures;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.MiscellaneousCommands.ShowDuplicates;

internal class ShowDuplicatesView : ViewBase<ShowDuplicatesViewModel>
{
    public override void Display(ShowDuplicatesViewModel viewModel)
    {
        foreach (DuplicateGroup fileGroup in viewModel.FileGroups)
        {
            foreach (string filePath in fileGroup.FilePaths)
                CustomConsole.WriteLine(filePath);

            DataSize sizeShort = fileGroup.FileSize;
            string sizeLong = fileGroup.FileSize.ToString(DataSizeUnit.Byte);
            FileHash fileHash = fileGroup.FileHash;
            CustomConsole.WriteLine(ConsoleColor.DarkGray, $"{sizeShort} ({sizeLong}) - {fileHash}");

            Console.WriteLine();
        }
        
        WriteValue("Duplicates", viewModel.DuplicateCount.ToString("N0"));
        WriteValue("Total size", viewModel.TotalSize.ToString("D"));
        Console.WriteLine();
    }
}