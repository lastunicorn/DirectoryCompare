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
using System.Linq;

namespace DustInTheWind.ConsoleFramework
{
    public class Arguments
    {
        public string Command { get; }

        public List<Argument> Values { get; } = new List<Argument>();

        public int Count => Values.Count;

        public bool IsEmpty => Values.Count == 0;

        public Argument this[int index]
        {
            get
            {
                if (index < 0 || index >= Values.Count)
                    return Argument.Empty;

                return Values[index];
            }
        }

        public Argument this[string name] => Values.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.InvariantCulture));

        public Arguments(IReadOnlyList<string> args)
        {
            if (args == null || args.Count == 0)
                return;

            Command = args[0];

            ArgumentsEnumerator argumentsEnumerator = new ArgumentsEnumerator(args.Skip(1));

            while (argumentsEnumerator.MoveNext())
            {
                Values.Add(argumentsEnumerator.Current);
            }
        }

        public IEnumerable<Argument> GetAnonymousArguments()
        {
            return Values.Where(x => x.Name == null);
        }

        public string GetStringValue(string name)
        {
            Argument argument = Values.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.InvariantCulture));

            if (argument.IsEmpty)
                return null;

            return argument.Value ?? argument.Name;
        }

        public string GetStringValue(int index)
        {
            if (index < 0 || index >= Values.Count)
                return null;

            Argument argument = Values[index];

            if (argument.IsEmpty)
                return null;

            return argument.Value ?? argument.Name;
        }

        public bool GetBoolValue(int index)
        {
            if (index < 0 || index >= Values.Count)
                return false;

            Argument argument = Values[index];

            return !argument.IsEmpty;
        }

        public bool GetBoolValue(string name)
        {
            Argument argument = Values.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.InvariantCulture));
            return !argument.IsEmpty;
        }
    }
}