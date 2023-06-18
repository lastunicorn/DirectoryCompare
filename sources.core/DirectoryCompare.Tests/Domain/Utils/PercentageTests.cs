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

using DustInTheWind.DirectoryCompare.Domain.Utils;
using FluentAssertions;
using Xunit;

namespace DustInTheWind.DirectoryCompare.Tests.Domain.Utils;

public class PercentageTests
{
    [Theory]
    [InlineData(50, 0)]
    [InlineData(150, 100)]
    [InlineData(100, 50)]
    [InlineData(51, 1)]
    public void Test(int underlyingValue, float expectedPercentageValue)
    {
        Percentage percentage = new(50, 150)
        {
            UnderlyingValue = underlyingValue
        };

        percentage.Value.Should().Be(expectedPercentageValue);
    }
}