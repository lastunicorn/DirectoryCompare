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
using System.Collections.Generic;
using DustInTheWind.ConsoleFramework;
using NUnit.Framework;

namespace DustInTheWind.DirectoryCompare.Tests.ConsoleFramework
{
    [TestFixture]
    public class ArgumentsEnumeratorTests
    {
        [Test]
        public void HavingNullChunkList_WhenEnumeratingArguments_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new ArgumentsEnumerator(null);
            });
        }

        [Test]
        public void HavingEmptyChunkList_WhenEnumeratingArguments_NoArgumentsAreEnumerated()
        {
            string[] chunks = new string[0];
            Argument[] expected = new Argument[0];
            PerformTest(chunks, expected);
        }

        [Test]
        public void HavingChunkListWithOneNullItem_WhenEnumeratingArguments_NoArgumentsAreEnumerated()
        {
            string[] chunks = { null };
            Argument[] expected = new Argument[0];
            PerformTest(chunks, expected);
        }

        [Test]
        public void HavingChunkListWithOneAnonymousArgument_WhenEnumeratingArguments_OneAnonymousArgumentIsEnumerated()
        {
            string[] chunks = { "value1" };
            Argument[] expected = {
                new Argument(null, "value1")
            };
            PerformTest(chunks, expected);
        }

        [Test]
        public void HavingChunkListWithOneNamedArgument_WhenEnumeratingArguments_OneNamedArgumentIsEnumerated()
        {
            string[] chunks = { "-param1", "value1" };
            Argument[] expected = {
                new Argument("param1", "value1")
            };
            PerformTest(chunks, expected);
        }

        [Test]
        public void HavingChunkListWithOneAnonymousBeforeOneNamedArguments_WhenEnumeratingArguments_OneAnonymousAndOneNamedArgumentIsEnumerated()
        {
            string[] chunks = { "value1", "-param2", "value2" };
            Argument[] expected = {
                new Argument(null, "value1"),
                new Argument("param2", "value2")
            };
            PerformTest(chunks, expected);
        }

        [Test]
        public void HavingChunkListWithOneNamedBeforeOneAnonymousArguments_WhenEnumeratingArguments_OneNamedAndOneAnonymousArgumentIsEnumerated()
        {
            string[] chunks = { "-param1", "value1", "value2" };
            Argument[] expected = {
                new Argument("param1", "value1"),
                new Argument(null, "value2")
            };
            PerformTest(chunks, expected);
        }

        [Test]
        public void HavingChunkListWithOneNameOnlyArgumentAtTheEnd_WhenEnumeratingArguments_TheLastArgumentIsEnumeratedCorrectly()
        {
            string[] chunks = { "-param1", "value1", "-param2" };
            Argument[] expected = {
                new Argument("param1", "value1"),
                new Argument("param2", null)
            };
            PerformTest(chunks, expected);
        }

        [Test]
        public void HavingChunkListWithOneNameOnlyArgumentAtTheBeginning_WhenEnumeratingArguments_TheFirstArgumentIsEnumeratedCorrectly()
        {
            string[] chunks = { "-param1", "-param2", "value2" };
            Argument[] expected = {
                new Argument("param1", null),
                new Argument("param2", "value2")
            };
            PerformTest(chunks, expected);
        }

        private static void PerformTest(IEnumerable<string> chunks, IEnumerable<Argument> expected)
        {
            ArgumentsEnumerator argumentsEnumerator = new ArgumentsEnumerator(chunks);
            IEnumerable<Argument> arguments = EnumerateAll(argumentsEnumerator);
            Assert.That(arguments, Is.EqualTo(expected));
        }

        private static IEnumerable<Argument> EnumerateAll(ArgumentsEnumerator enumerator)
        {
            while (enumerator.MoveNext())
                yield return enumerator.Current;
        }
    }
}