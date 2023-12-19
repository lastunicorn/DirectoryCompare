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

using System.Collections.ObjectModel;

namespace DustInTheWind.DirectoryCompare.Cli.Application.SnapshotArea.CreateSnapshot.Crawling;

internal class IncludeExcludeMatchCollection : Collection<IncludeExcludeMatch>
{
    public bool IsExactMatch { get; private set; }

    public bool IsIntermediateMatch { get; private set; }

    public IncludeExcludeRuleCollection NextRules { get; } = new();

    public IncludeExcludeMatchCollection()
    {
    }

    public IncludeExcludeMatchCollection(IEnumerable<IncludeExcludeMatch> matches)
        : base(new List<IncludeExcludeMatch>(matches))
    {
    }

    public void Analyze(bool includeRules = true)
    {
        IsExactMatch = Items.Count == 0;
        IsIntermediateMatch = false;
        NextRules.Clear();

        foreach (IncludeExcludeMatch match in Items)
        {
            switch (match.MatchType)
            {
                case MatchType.Exact:
                    IsExactMatch = true;
                    break;

                case MatchType.Intermediate:
                    IsIntermediateMatch = true;
                    break;

                case MatchType.None:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (includeRules)
                NextRules.AddRange(match.NextRules);
        }
    }
}