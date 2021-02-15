using System;
using System.IO;
using System.Text.RegularExpressions;

namespace DustInTheWind.DirectoryCompare.JFiles
{
    public class SnapshotFilePath
    {
        private static readonly Regex FileNameRegex = new Regex(@"^([1-9]\d*)\s(0[1-9]|1[0-2])\s(0[1-9]|[12]\d|3[01])\s([0-5]\d)([0-5]\d)([0-5]\d)$", RegexOptions.Singleline);

        private readonly string filePath;
        private readonly string fileName;

        public DateTime CreationTime { get; }

        public SnapshotFilePath()
        {
            CreationTime = DateTime.UtcNow;
            fileName = $"{CreationTime:yyyy MM dd HHmmss}.json";
        }

        public SnapshotFilePath(DateTime creationTime)
        {
            CreationTime = creationTime;
            fileName = $"{CreationTime:yyyy MM dd HHmmss}.json";
        }

        public SnapshotFilePath(DateTime creationTime, string rootPath)
        {
            CreationTime = creationTime;
            fileName = $"{CreationTime:yyyy MM dd HHmmss}.json";
            filePath = Path.Combine(rootPath, fileName);
        }

        public SnapshotFilePath(string filePath)
        {
            this.filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));

            fileName = Path.GetFileName(filePath);
            CreationTime = ExtractCreationTime();
        }

        private DateTime ExtractCreationTime()
        {
            string dateString = Path.GetFileNameWithoutExtension(fileName);
            Match match = FileNameRegex.Match(dateString);

            if (!match.Success)
                throw new ArgumentException("Invalid snapshot file name", nameof(filePath));

            int year = int.Parse(match.Groups[1].Value);
            int month = int.Parse(match.Groups[2].Value);
            int day = int.Parse(match.Groups[3].Value);
            int hour = int.Parse(match.Groups[4].Value);
            int minute = int.Parse(match.Groups[5].Value);
            int second = int.Parse(match.Groups[6].Value);

            return new DateTime(year, month, day, hour, minute, second);
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