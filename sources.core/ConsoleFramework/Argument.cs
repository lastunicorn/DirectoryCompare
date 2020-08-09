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

namespace DustInTheWind.ConsoleFramework
{
    public struct Argument
    {
        public static Argument Empty { get; } = new Argument(null, null);

        public string Name { get; }

        public string Value { get; }

        public bool IsEmpty => Name == null && Value == null;

        public bool HasName => !string.IsNullOrEmpty(Name);
        
        public Argument(string value)
            : this(null, value)
        {
        }

        public Argument(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}