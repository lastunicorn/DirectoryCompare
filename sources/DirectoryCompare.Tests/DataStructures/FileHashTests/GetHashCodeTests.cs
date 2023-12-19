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

using DustInTheWind.DirectoryCompare.DataStructures;
using FluentAssertions;
using Xunit;

namespace DustInTheWind.DirectoryCompare.Tests.DataStructures.FileHashTests;

public class GetHashCodeTests
{
    [Fact]
    public void HavingTwoInstancesWithSameValue_WhenComparingTheHashCode_ThenReturnsTrue()
    {
        FileHash fileHash1 = FileHash.Parse("1B2M2Y8AsgTpgAmY7PhCfg==");
        int hashCode1 = fileHash1.GetHashCode();
        
        FileHash fileHash2 = FileHash.Parse("1B2M2Y8AsgTpgAmY7PhCfg==");
        int hashCode2 = fileHash2.GetHashCode();
        
        bool actual = hashCode1.Equals(hashCode2);

        actual.Should().BeTrue();
    }
}