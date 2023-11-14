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
using DustInTheWind.DirectoryCompare.Cli.Application.PotArea.PresentPots;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.PotCommands.DisplayPots;

internal class DisplayPotsCommandView : ViewBase<DisplayPotsCommand>
{
    public override void Display(DisplayPotsCommand command)
    {
        bool hasPots = command.Pots is { Count: > 0 };

        if (hasPots)
            DisplayPots(command.Pots);
        else
            WriteInfo("There are no Pots.");
    }

    private static void DisplayPots(List<PotDto> pots)
    {
        foreach (PotDto pot in pots)
            DisplayPot(pot);
    }

    private static void DisplayPot(PotDto pot)
    {
        string guid = pot.Guid.ToString()[..8];
        CustomConsole.Write(guid);
        CustomConsole.Write(" ");

        CustomConsole.WriteEmphasized(pot.Name);
        CustomConsole.Write(" ");

        CustomConsole.Write(ConsoleColor.DarkGray, pot.Path);
        CustomConsole.WriteLine();
    }
}