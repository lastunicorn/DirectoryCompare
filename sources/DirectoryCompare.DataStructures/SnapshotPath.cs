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

using System.Collections;

namespace DustInTheWind.DirectoryCompare.DataStructures;

/// <summary>
/// Represents a path inside a snapshot.
/// </summary>
public readonly struct SnapshotPath : IEnumerable<string>
{
    private readonly string path;

    public SnapshotPath(string path)
    {
        this.path = path;
    }

    public bool IsEmpty => string.IsNullOrEmpty(path);

    public IEnumerable<string> EnumerateDirectories()
    {
        using IEnumerator<string> enumerator = GetEnumerator();

        string previousPart = null;

        while (enumerator.MoveNext())
        {
            if (previousPart == null)
            {
                previousPart = enumerator.Current;
            }
            else
            {
                yield return previousPart;
                previousPart = enumerator.Current;
            }
        }
    }

    public IEnumerable<string> Enumerate()
    {
        using IEnumerator<string> enumerator = GetEnumerator();

        while (enumerator.MoveNext())
            yield return enumerator.Current;
    }

    public IEnumerator<string> GetEnumerator()
    {
        return new SnapshotPathEnumerator(this);
    }

    public override string ToString()
    {
        if (path == null)
            return string.Empty;

        return path.StartsWith("/")
            ? path
            : "/" + path;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public static implicit operator SnapshotPath(string path)
    {
        return new SnapshotPath(path);
    }

    public static implicit operator string(SnapshotPath path)
    {
        return path.ToString();
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

    private class SnapshotPathEnumerator : IEnumerator<string>
    {
        private readonly string[] parts;
        private readonly bool isRooted;
        private int index;

        public string Current { get; private set; }

        object IEnumerator.Current => Current;

        public SnapshotPathEnumerator(SnapshotPath snapshotPath)
        {
            parts = snapshotPath.path?.Split('/');
            isRooted = snapshotPath.path?.StartsWith("/") ?? false;

            index = isRooted ? 1 : 0;
        }

        public bool MoveNext()
        {
            if (parts == null || index == parts.Length)
            {
                Current = null;
                return false;
            }

            Current = parts[index].Trim();
            index++;

            return true;
        }

        public void Reset()
        {
            index = isRooted ? 1 : 0;
        }

        public void Dispose()
        {
        }
    }
}