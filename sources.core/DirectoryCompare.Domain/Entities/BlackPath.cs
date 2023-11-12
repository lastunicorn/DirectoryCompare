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

namespace DustInTheWind.DirectoryCompare.Domain.Entities;

public readonly struct BlackPath
{
    private readonly bool isRooted;
    private readonly bool isDirectoryOnly;
    private readonly string[] parts;

    public BlackPath(string pattern)
    {
        string trimmedPattern = pattern.TrimStart();
        isRooted = trimmedPattern.StartsWith("/");
        isDirectoryOnly = trimmedPattern.EndsWith("/");
        parts = pattern
            .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
            .ToArray();
    }

    public bool Matches(HItem hItem)
    {
        int index = parts.Length - 1;
        HItem currentHItem = hItem;

        while (index >= 0)
        {
            if (currentHItem == null)
                return false;

            bool isMatch = currentHItem.Name == parts[index] && (index != parts.Length - 1 || !isDirectoryOnly || currentHItem is HDirectory);

            if (isMatch)
                index--;
            else
                index = parts.Length - 1;

            currentHItem = currentHItem.Parent;
        }

        if (isRooted && currentHItem != null)
            return false;

        return true;
    }
}