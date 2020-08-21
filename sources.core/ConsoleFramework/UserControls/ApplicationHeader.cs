// DirectoryCompare
// Copyright (C) 2017-2020 Dust in the Wind
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

using System;
using System.Reflection;
using DustInTheWind.ConsoleTools;

namespace DustInTheWind.ConsoleFramework.UserControls
{
    public class ApplicationHeader
    {
        private readonly Version version;

        public ApplicationHeader()
        {
            version = GetVersion();
        }

        public void Display()
        {
            CustomConsole.WriteLine($"Directory Compare ver {version?.ToString(3)}");
            CustomConsole.WriteLine(new string('=', 79));
            CustomConsole.WriteLine();
        }

        private static Version GetVersion()
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            AssemblyName assemblyName = assembly?.GetName();
            return assemblyName?.Version;
        }
    }
}