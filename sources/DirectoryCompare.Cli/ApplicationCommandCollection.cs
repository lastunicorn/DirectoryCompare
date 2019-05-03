// DirectoryCompare
// Copyright (C) 2017 Dust in the Wind
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

using DustInTheWind.DirectoryCompare.Cli.Commands;

namespace DustInTheWind.DirectoryCompare.Cli
{
    internal class ApplicationCommandCollection : CommandCollection
    {
        public ApplicationCommandCollection()
        {
            Commands.Add("read-disk", new ReadDiskCommand());
            Commands.Add("read-file", new ReadFileCommand());
            Commands.Add("verify-disk", new VerifyDiskCommand());
            Commands.Add("compare-disks", new CompareDisksCommand());
            Commands.Add("compare-files", new CompareFilesCommand());
            Commands.Add("find-duplicates", new FindDuplicatesCommand());
            Commands.Add("remove-duplicates", new RemoveDuplicatesCommand());
        }
    }
}