// DirectoryCompare
// Copyright (C) 2017-2019 Dust in the Wind
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
using System;

namespace DustInTheWind.DirectoryCompare.Cli
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                /**
                 * dc read
                 */

                //args = new[] { "read-disk", @"c:\temp1", @"c:\temp\file1.json" };
                //args = new[] { "read-disk", @"c:\temp2", @"c:\temp\file2.json" };
                //args = new[] { "read-disk", @"c:\temp", @"c:\temp1\temp.json" };
                //args = new[] { "read-file", @"c:\temp1\temp.json" };
                //args = new[] { "verify-disk", @"c:\temp1", @"c:\temp\file1.json" };
                //args = new[] { "compare-disks", @"c:\temp1", @"c:\temp2" };
                //args = new[] { "compare-files", @"c:\temp\file1.json", @"c:\temp\file2.json" };

                ConsoleApplication consoleApplication = new ConsoleApplication();
                consoleApplication.Initialize();

                consoleApplication.Run(args);
            }
            catch (Exception ex)
            {
                CustomConsole.WriteLineError(ex);
            }
        }
    }
}