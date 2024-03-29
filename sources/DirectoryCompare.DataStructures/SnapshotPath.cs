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

namespace DustInTheWind.DirectoryCompare.DataStructures;

public readonly struct SnapshotPath
{
    private readonly string path;

    public SnapshotPath(string path)
    {
        this.path = path;
    }

    public bool IsEmpty => string.IsNullOrEmpty(path);

    public IEnumerable<string> Enumerate()
    {
        if (path == null)
            yield break;

        string[] parts = path.Split('/');

        bool isRooted = path.StartsWith("/");
        int startIndex = isRooted ? 1 : 0;

        for (int i = startIndex; i < parts.Length; i++)
        {
            string part = parts[i].Trim();
            yield return part;
        }
    }

    public override string ToString()
    {
        return path;
    }

    public static implicit operator SnapshotPath(string path)
    {
        return new SnapshotPath(path);
    }

    public static implicit operator string(SnapshotPath path)
    {
        return path.path;
    }

    public static SnapshotPath operator +(SnapshotPath path1, SnapshotPath path2)
    {
        if (path1.path == "/")
            return path1.path + path2.path?.TrimStart('/');

        List<string> parts = new(2);

        string part1 = path1.path?.TrimEnd('/');
        if (part1 != null)
            parts.Add(part1);

        string part2 = path2.path;
        if (part2 != null)
            parts.Add(part2);

        string result = string.Join('/', parts);

        return new SnapshotPath(result);
    }
}