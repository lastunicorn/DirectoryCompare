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

using DustInTheWind.DirectoryCompare.DataStructures;

namespace DustInTheWind.DirectoryCompare.Domain.Entities;

public class Snapshot : HDirectory
{
    public Guid Id { get; set; }

    public string OriginalPath { get; set; }

    public DateTime CreationTime { get; set; }

    public Snapshot()
        : base(string.Empty)
    {
    }

    public HDirectory GetDirectory(SnapshotPath path)
    {
        HDirectory hDirectory = this;

        IEnumerable<string> names = path.Enumerate();

        foreach (string name in names)
        {
            hDirectory = hDirectory.GetChildDirectory(name);

            if (hDirectory == null)
                throw new Exception($"Directory could not be found: {path}");
        }

        return hDirectory;
    }

    public HFile GetFile(SnapshotPath path)
    {
        HDirectory hDirectory = this;
        IEnumerable<string> names = path.Enumerate();
        string previousName = null;

        foreach (string name in names)
        {
            if (previousName != null)
            {
                hDirectory = hDirectory.GetChildDirectory(previousName);

                if (hDirectory == null)
                    return null;
            }

            previousName = name;
        }

        return hDirectory.GetChildFile(previousName);
    }

    public override string ToString()
    {
        return $"Snapshot: {Id:D)}; Path: {OriginalPath}";
    }
}