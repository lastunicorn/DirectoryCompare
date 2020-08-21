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

using System;
using System.Collections;
using System.Collections.Generic;

namespace DustInTheWind.ConsoleFramework
{
    public sealed class ArgumentsEnumerator : IEnumerator<Argument>
    {
        private readonly IEnumerable<string> args;
        private IEnumerator<string> enumerator;
        private string previousName;

        public ArgumentsEnumerator(IEnumerable<string> args)
        {
            this.args = args ?? throw new ArgumentNullException(nameof(args));

            enumerator = args.GetEnumerator();
        }

        public bool MoveNext()
        {
            while (true)
            {
                // move to next chunk

                bool isMoved = enumerator.MoveNext();

                if (!isMoved)
                {
                    // if no more chunks, publish last argument and close.

                    if (previousName != null)
                    {
                        PublishArgument();
                        return true;
                    }

                    return false;
                }

                // ignore null chunks
                if (enumerator.Current == null)
                    continue;

                string chunk = enumerator.Current;

                bool isNewArgument = chunk.StartsWith("-");

                if (isNewArgument)
                {
                    if (previousName != null)
                    {
                        PublishArgument();
                        StoreParameterName(chunk);

                        return true;
                    }

                    StoreParameterName(chunk);
                }
                else
                {
                    PublishArgument(chunk);
                    return true;
                }
            }
        }

        private void PublishArgument(string value = null)
        {
            Current = new Argument(previousName, value);
            previousName = null;
        }

        private void StoreParameterName(string chunk)
        {
            previousName = chunk.TrimStart('-');
        }

        public void Reset()
        {
            enumerator?.Dispose();
            enumerator = args.GetEnumerator();
            previousName = null;
        }

        public Argument Current { get; set; }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            enumerator?.Dispose();
        }
    }
}