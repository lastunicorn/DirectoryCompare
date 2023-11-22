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
using DustInTheWind.DirectoryCompare.DataStructures;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.PotCommands.DisplayPots;

internal class PotViewControl
{
    public Guid Guid { get; set; }

    public string Name { get; set; }

    public string Path { get; set; }

    public DataSize Size { get; set; }

    public bool DisplaySize { get; set; }

    public void Display()
    {
        string guid = Guid.ToString()[..8];
        CustomConsole.Write(guid);
        CustomConsole.Write(" ");

        CustomConsole.WriteEmphasized(Name);
        CustomConsole.Write(" ");

        if (DisplaySize) 
            CustomConsole.Write(ConsoleColor.DarkGray, $"({Size}) ");

        CustomConsole.Write(ConsoleColor.DarkGray, Path);
        CustomConsole.WriteLine();
    }
}