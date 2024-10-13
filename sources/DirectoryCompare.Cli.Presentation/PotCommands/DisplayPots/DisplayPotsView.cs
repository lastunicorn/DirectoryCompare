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

using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.DirectoryCompare.Ports.ConfigAccess;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.PotCommands.DisplayPots;

internal class DisplayPotsView : ViewBase<PotsViewModel>
{
    private readonly IConfig config;

    public DisplayPotsView(IConfig config)
    {
        this.config = config ?? throw new ArgumentNullException(nameof(config));
    }
    
    public override void Display(PotsViewModel potsViewModel)
    {
        bool hasPots = potsViewModel.Pots is { Count: > 0 };

        if (hasPots)
            DisplayPots(potsViewModel);
        else
            WriteInfo("There are no Pots.");
    }

    private void DisplayPots(PotsViewModel potsViewModel)
    {
        PotsDataGrid potsDataGrid = new()
        {
            DataSizeFormat = config.DataSizeFormat.ToPresentationModel()
        };
        potsDataGrid.AddPots(potsViewModel);

        potsDataGrid.Display();
    }
}