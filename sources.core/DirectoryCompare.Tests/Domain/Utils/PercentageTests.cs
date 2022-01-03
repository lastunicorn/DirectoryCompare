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
using NUnit.Framework;

namespace DustInTheWind.DirectoryCompare.Tests.Domain.Utils
{
    [TestFixture]
    public class PercentageTests
    {
        [TestCase(50, 0)]
        [TestCase(150, 100)]
        [TestCase(100, 50)]
        [TestCase(51, 1)]
        public void Test(int underlyingValue, float percentageValue)
        {
            Percentage percentage = new(50, 150)
            {
                UnderlyingValue = underlyingValue
            };

            Assert.That(percentage.Value, Is.EqualTo(percentageValue));
        }
    }
}