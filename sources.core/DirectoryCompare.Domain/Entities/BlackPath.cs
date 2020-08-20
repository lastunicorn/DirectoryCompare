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

using System;
using System.Linq;

namespace DustInTheWind.DirectoryCompare.Domain.Entities
{
    public struct BlackPath
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
                .Reverse()
                .ToArray();
        }

        public bool Matches(HItem hItem)
        {
            int index = 0;
            HItem currentHItem = hItem;

            if (isDirectoryOnly && !(currentHItem is HDirectory))
                return false;

            while (index < parts.Length)
            {
                string currentPart = parts[index];

                if (currentHItem == null || currentHItem.Name != currentPart)
                    return false;

                index++;
                currentHItem = currentHItem.Parent;
            }

            if (isRooted && currentHItem != null)
                return false;

            return true;
        }
    }
}