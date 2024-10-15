// Directory Compare
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

using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.DirectoryCompare.Cli.Application.MiscellaneousArea.FindChange;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.MiscellaneousCommands.FindChange;

internal class FindChangeView : IView<FindChangeViewModel>
{
    public void Display(FindChangeViewModel viewModel)
    {
        Console.WriteLine(viewModel.Path);
        
        int index = 0;
        foreach (HFileState fileState in viewModel.Changes)
        {
            string text = $"{index:00} {fileState.SnapshotDateTime}";

            if (!fileState.FileExists)
            {
                text += " - does not exist";
            }
            else
            {
                if (fileState.FileIsChanged)
                    text += " - changed";
            }
            
            Console.WriteLine(text);

            index++;
        }
    }
}