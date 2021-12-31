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
using System.IO;
using System.Text.RegularExpressions;

namespace DustInTheWind.DirectoryCompare.JFiles
{
    public class SnapshotFilePath
    {
        private static readonly Regex FileNameRegex = new Regex(@"^([1-9]\d*)\s(0[1-9]|1[0-2])\s(0[1-9]|[12]\d|3[01])\s([0-5]\d)([0-5]\d)([0-5]\d).json$", RegexOptions.Singleline);

        private readonly string filePath;

        public DateTime CreationTime { get; }

        public SnapshotFilePath(DateTime creationTime)
        {
            CreationTime = creationTime;
            filePath = $"{CreationTime:yyyy MM dd HHmmss}.json";
        }

        public SnapshotFilePath(DateTime creationTime, string rootPath)
        {
            CreationTime = creationTime;
            string fileName = $"{CreationTime:yyyy MM dd HHmmss}.json";
            filePath = Path.Combine(rootPath, fileName);
        }

        public SnapshotFilePath(string filePath)
        {
            this.filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));

            string fileName = Path.GetFileName(filePath);
            
            if (string.IsNullOrEmpty(fileName))
                throw new FileNameNotSpecifiedException();

            Match match = FileNameRegex.Match(fileName);

            if (!match.Success)
                throw new InvalidSnapshotFileNameException();
            
            CreationTime = ExtractCreationTime(match);
        }

        private DateTime ExtractCreationTime(Match match)
        {
            int year = int.Parse(match.Groups[1].Value);
            int month = int.Parse(match.Groups[2].Value);
            int day = int.Parse(match.Groups[3].Value);
            int hour = int.Parse(match.Groups[4].Value);
            int minute = int.Parse(match.Groups[5].Value);
            int second = int.Parse(match.Groups[6].Value);

            return new DateTime(year, month, day, hour, minute, second);
        }

        public override string ToString()
        {
            return filePath;
        }

        public static implicit operator string(SnapshotFilePath snapshotFilePath)
        {
            return snapshotFilePath.filePath;
        }

        public static implicit operator SnapshotFilePath(string filePath)
        {
            return new SnapshotFilePath(filePath);
        }
    }
}