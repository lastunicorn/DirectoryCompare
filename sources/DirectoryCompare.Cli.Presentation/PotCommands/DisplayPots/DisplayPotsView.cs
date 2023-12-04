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

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.PotCommands.DisplayPots;

internal class DisplayPotsView : ViewBase<PotsViewModel>
{
    public override void Display(PotsViewModel potsViewModel)
    {
        bool hasPots = potsViewModel.Pots is { Count: > 0 };

        if (hasPots)
            DisplayPots(potsViewModel);
        else
            WriteInfo("There are no Pots.");
    }

    private static void DisplayPots(PotsViewModel potsViewModel)
    {
        foreach (PotViewModel pot in potsViewModel.Pots)
            DisplayPot(pot, potsViewModel.DisplaySizes);

        if (potsViewModel.DisplaySizes)
        {
            CustomConsole.WriteLine();
            CustomConsole.WriteLine($"Total Size: {potsViewModel.TotalSize}");
        }
    }

    private static void DisplayPot(PotViewModel pot, bool displaySizes)
    {
        PotViewControl potViewControl = new()
        {
            Guid = pot.Guid,
            Name = pot.Name,
            Path = pot.Path,
            Size = pot.Size,
            DisplaySize = displaySizes,
            HasPathFilters = pot.HasPathFilters
        };

        potViewControl.Display();
    }
}