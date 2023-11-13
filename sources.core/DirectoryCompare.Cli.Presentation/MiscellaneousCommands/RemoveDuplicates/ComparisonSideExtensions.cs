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

using DustInTheWind.DirectoryCompare.Cli.Application.MiscellaneousArea.RemoveDuplicates;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.MiscellaneousCommands.RemoveDuplicates;

internal static class ComparisonSideExtensions
{
    public static ComparisonSide ToBusiness(this ComparisonSidePm comparisonSide)
    {
        return comparisonSide switch
        {
            ComparisonSidePm.Left => ComparisonSide.Left,
            ComparisonSidePm.Right => ComparisonSide.Right,
            _ => throw new ArgumentOutOfRangeException(nameof(comparisonSide), "Invalid value for comparison side.")
        };
    }
}