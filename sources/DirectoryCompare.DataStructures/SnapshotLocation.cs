﻿// DirectoryCompare
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

using System.ComponentModel;

namespace DustInTheWind.DirectoryCompare.DataStructures;

[TypeConverter(typeof(SnapshotLocationConverter))]
public readonly struct SnapshotLocation
{
    private const char SnapshotIndicator = '~';
    private const char PathIndicator = '/';

    private readonly string rawValue;

    public string PotName { get; }

    public int? SnapshotIndex { get; }

    public DateTime? SnapshotDate { get; }

    public string InternalPath { get; }

    public SnapshotLocation(string value)
    {
        rawValue = value;

        if (value == null)
        {
            PotName = null;
            SnapshotIndex = null;
            SnapshotDate = null;
            InternalPath = null;
        }
        else
        {
            Tuple<string, string> parts = SplitByFirstOccurrence(value, PathIndicator);
            InternalPath = parts.Item2;

            parts = SplitByFirstOccurrence(parts.Item1, SnapshotIndicator);
            PotName = parts.Item1;

            if (int.TryParse(parts.Item2, out int snapshotIndex))
            {
                SnapshotIndex = snapshotIndex;
                SnapshotDate = null;
            }
            else if (DateTime.TryParse(parts.Item2, out DateTime snapshotDate))
            {
                SnapshotIndex = null;
                SnapshotDate = snapshotDate;
            }
            else
            {
                SnapshotIndex = null;
                SnapshotDate = null;
            }
        }
    }

    private static Tuple<string, string> SplitByFirstOccurrence(string text, char c)
    {
        int pos = text.IndexOf(c);
        string value1;
        string value2;

        if (pos >= 0)
        {
            value1 = text.Substring(0, pos);
            value2 = text.Substring(pos + 1);
        }
        else
        {
            value1 = text;
            value2 = null;
        }

        return new Tuple<string, string>(value1, value2);
    }

    public override string ToString()
    {
        return rawValue;
    }

    public static implicit operator string(SnapshotLocation snapshotLocation)
    {
        return snapshotLocation.rawValue;
    }

    public static implicit operator SnapshotLocation(string path)
    {
        return new SnapshotLocation(path);
    }
}