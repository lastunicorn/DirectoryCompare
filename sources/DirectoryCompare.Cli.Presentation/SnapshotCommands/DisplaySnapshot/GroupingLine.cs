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

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.SnapshotCommands.DisplaySnapshot;

internal class GroupingLine
{
    private readonly int lineCount;
    private readonly GroupingLine previousGroupingLine;
    private int index;
    private const ConsoleColor ForegroundColor = ConsoleColor.DarkGray;

    private bool IsFirstLine => index == 0;

    private bool IsLastLine => index == lineCount - 1;

    private bool IsFinished => index >= lineCount;

    public GroupingLine(int lineCount, GroupingLine previousGroupingLine)
    {
        this.lineCount = lineCount;
        this.previousGroupingLine = previousGroupingLine;
    }

    public void DisplayNext()
    {
        previousGroupingLine?.DisplayMiddle();

        if (IsFirstLine)
        {
            string text = lineCount == 1
                ? " ∙ "
                : " ┌ ";

            CustomConsole.Write(ForegroundColor, text);
        }
        else if (IsLastLine)
        {
            CustomConsole.Write(ForegroundColor, " └ ");
        }
        else if (IsFinished)
        {
            CustomConsole.Write(ForegroundColor, "   ");
        }
        else
        {
            CustomConsole.Write(ForegroundColor, " │ ");
        }

        index++;
    }

    private void DisplayMiddle()
    {
        previousGroupingLine?.DisplayMiddle();

        string text = lineCount > 1 && !IsFinished
            ? " │ "
            : "   ";

        CustomConsole.Write(ConsoleColor.DarkGray, text);
    }
}