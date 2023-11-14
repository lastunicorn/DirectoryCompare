// VeloCity
// Copyright (C) 2022-2023 Dust in the Wind
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
using DustInTheWind.ConsoleTools.Controls.InputControls;
using DustInTheWind.DirectoryCompare.Ports.UserAccess;

namespace DustInTheWind.DirectoryCompare.UserAccess;

public class UserInterface : IUserInterface
{
    public Task<bool> ConfirmToDelete(PotDeletionRequest request)
    {
        CustomConsole.WriteLineEmphasized("Deleting pot");
        WriteLabeledValue("Pot Name", request.PotName);
        WriteLabeledValue("Pot Id", request.PotId);
        Console.WriteLine();

        return Task.Run(() =>
        {
            YesNoAnswer answer = YesNoQuestion.QuickRead("Are you sure you want to delete the pot?", YesNoAnswer.Yes);
            return answer == YesNoAnswer.Yes;
        });
    }

    private static void WriteLabeledValue(string label, object value)
    {
        CustomConsole.Write("  ");
        CustomConsole.WriteEmphasized(label + ": ");
        CustomConsole.WriteLine(ConsoleColor.DarkGray, value);
    }
}