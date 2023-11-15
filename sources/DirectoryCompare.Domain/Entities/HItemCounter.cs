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

using DustInTheWind.DirectoryCompare.DataStructures;

namespace DustInTheWind.DirectoryCompare.Domain.Entities;

public class HItemCounter
{
    private readonly HDirectory rootDirectory;

    public DataSize DataSize { get; private set; }

    public int FileCount { get; private set; }

    public int DirectoryCount { get; private set; }

    public HItemCounter(HDirectory rootDirectory)
    {
        this.rootDirectory = rootDirectory ?? throw new ArgumentNullException(nameof(rootDirectory));
    }

    public void Count()
    {
        FileCount = 0;
        DirectoryCount = 0;

        Count(rootDirectory);
    }

    private void Count(HDirectory directory)
    {
        FileCount += directory.Files.Count;
        DirectoryCount += directory.Directories.Count;

        foreach (HFile file in directory.Files) 
            DataSize += file.Size;

        foreach (HDirectory subdirectory in directory.Directories)
            Count(subdirectory);
    }
}