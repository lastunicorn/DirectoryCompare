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
using DustInTheWind.ConsoleTools.Controls.InputControls;
using DustInTheWind.DirectoryCompare.Ports.UserAccess;

namespace DustInTheWind.DirectoryCompare.UserAccess;

public class DeletePotUi : EnhancedConsole, IDeletePotUi
{
    public Task<bool> ConfirmToDelete(PotDeletionRequest request)
    {
        WithIndentation("Deleting pot", () =>
        {
            WriteValue("Pot Name", request.PotName);
            WriteValue("Pot Id", request.PotId);
        });

        Console.WriteLine();

        return Task.Run(() =>
        {
            YesNoAnswer answer = YesNoQuestion.QuickRead("Are you sure you want to delete the pot?", YesNoAnswer.Yes);
            return answer == YesNoAnswer.Yes;
        });
    }
}