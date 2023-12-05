// DirectoryCompare
// Copyright (C) 2017-2023 Dust in the Wind
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

namespace DustInTheWind.DirectoryCompare.FileSystemAccess;

internal class IncludeExcludeRule
{
    private readonly string[] items;
    private readonly bool isRooted;
    

    public IncludeExcludeRule(string value)
    {
        if (value != null)
        {
            items = value.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            isRooted = value.TrimStart().StartsWith("/");
        }
    }

    private IncludeExcludeRule(string[] items, bool isRooted)
    {
        this.items = items;
        this.isRooted = isRooted;
    }

    public IncludeExcludeMatchResult Match(string name)
    {
        bool isMatched = ComputeMatch(name);
        IEnumerable<IncludeExcludeRule> nextRules = ComputeNextRule(isMatched);

        return new IncludeExcludeMatchResult
        {
            IsMatch = isMatched,
            NextRules = nextRules
        };
    }

    private bool ComputeMatch(string name)
    {
        if (name == null)
            return false;

        if (items == null)
            return false;

        if (items.Length == 0)
            return false;

        if (items[0] != name)
            return false;

        return true;
    }

    private IEnumerable<IncludeExcludeRule> ComputeNextRule(bool isMatched)
    {
        if (isMatched)
        {
            bool isLeaf = items.Length == 1;

            if (isRooted)
            {
                if (isLeaf)
                    yield break;
                
                yield return CreateNewWithoutFirstItem();
            }

            if (isLeaf)
                yield break;

            yield return CreateNewWithoutFirstItem();
            yield return this;
        }
        else
        {
            if (isRooted)
                yield break;

            yield return this;
        }
    }

    private IncludeExcludeRule CreateNewWithoutFirstItem()
    {
        string[] newItems = items.Skip(1).ToArray();
        return new IncludeExcludeRule(newItems, true);
    }
}