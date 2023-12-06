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

    public IncludeExcludeMatch Match(string name)
    {
        MatchType matchType = ComputeMatch(name);
        IEnumerable<IncludeExcludeRule> nextRules = ComputeNextRule(matchType);

        return new IncludeExcludeMatch
        {
            MatchType = matchType,
            NextRules = nextRules
        };
    }

    private MatchType ComputeMatch(string name)
    {
        if (name == null)
            return MatchType.None;

        if (items == null)
            return MatchType.None;

        if (items.Length == 0)
            return MatchType.None;

        if (items[0] != name)
            return MatchType.None;

        bool isLeaf = items.Length == 1;

        return isLeaf
            ? MatchType.Exact
            : MatchType.Intermediate;
    }

    private IEnumerable<IncludeExcludeRule> ComputeNextRule(MatchType matchType)
    {
        switch (matchType)
        {
            case MatchType.None:
                if (!isRooted)
                    yield return this;
                break;

            case MatchType.Exact:
                yield break;

            case MatchType.Intermediate:
                yield return CreateNewWithoutFirstItem();
                if (!isRooted)
                    yield return this;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(matchType), matchType, null);
        }
    }

    private IncludeExcludeRule CreateNewWithoutFirstItem()
    {
        string[] newItems = items.Skip(1).ToArray();
        return new IncludeExcludeRule(newItems, true);
    }
}