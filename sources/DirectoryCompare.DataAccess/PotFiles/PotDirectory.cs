// DirectoryCompare
// Copyright (C) 2017-2024 Dust in the Wind
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

using DustInTheWind.DirectoryCompare.DataAccess.PotFiles.BlacklistFileModel;
using DustInTheWind.DirectoryCompare.DataAccess.PotFiles.PotInfoFileModel;

namespace DustInTheWind.DirectoryCompare.DataAccess.PotFiles;

public class PotDirectory
{
    private const string SnapshotsDirectoryName = "snapshots";
    private JPotInfoFile jPotInfoFile;

    public string RootPath { get; private set; }

    public string FullPath { get; private set; }

    public JPotInfoFile InfoFile => jPotInfoFile ??= GetInfoFile();

    public Guid PotGuid
    {
        get
        {
            if (string.IsNullOrEmpty(FullPath))
                throw new Exception("Pot directory path is invalid.");

            string directoryName = Path.GetFileName(FullPath);

            if (directoryName == string.Empty)
            {
                string parentDirectoryPath = Path.GetDirectoryName(FullPath);
                directoryName = Path.GetFileName(parentDirectoryPath);
            }

            try
            {
                return new Guid(directoryName);
            }
            catch (Exception ex)
            {
                throw new Exception("Pot directory name is invalid.", ex);
            }
        }
    }

    public bool IsValid
    {
        get
        {
            bool directoryExists = Directory.Exists(FullPath);
            if (!directoryExists)
                return false;

            JPotInfo jPotInfo = InfoFile.Read();

            if (jPotInfo == null)
                return false;

            return true;
        }
    }

    private PotDirectory()
    {
    }

    public PotDirectory(string fullPath)
    {
        FullPath = fullPath;

        if (fullPath != null)
            RootPath = Path.GetDirectoryName(FullPath);
    }

    public static PotDirectory FromPotName(string potName, string storagePath)
    {
        if (potName == null) throw new ArgumentNullException(nameof(potName));

        PotDirectory potDirectory = Directory.GetDirectories(storagePath)
            .Select(x => new PotDirectory(x))
            .Where(x =>
            {
                JPotInfoFile jPotInfoFile = x.GetInfoFile();
                JPotInfo jPotInfo = jPotInfoFile.Read();
                return jPotInfo != null && jPotInfo.Name == potName;
            })
            .FirstOrDefault();

        return potDirectory ?? New(storagePath);
    }

    public static PotDirectory New(string storagePath)
    {
        return new PotDirectory
        {
            RootPath = storagePath
        };
    }

    public void Create()
    {
        if (IsValid)
            throw new PotDirectoryExistsException();

        for (int i = 0; i < 10000; i++)
        {
            Guid guid = Guid.NewGuid();
            string path = Path.Combine(RootPath, guid.ToString("D"));

            if (Directory.Exists(path))
                continue;

            Directory.CreateDirectory(path);
            FullPath = path;
            return;
        }

        throw new NameGenerationException();
    }

    public void Delete()
    {
        Directory.Delete(FullPath, true);
    }

    public long CalculateSize()
    {
        DirectoryInfo directoryInfo = new(FullPath);
        return CalculateSize(directoryInfo);
    }

    private static long CalculateSize(DirectoryInfo directoryInfo)
    {
        long fileSizes = directoryInfo.GetFiles().Sum(x => x.Length);
        long subdirSizes = directoryInfo.GetDirectories().Sum(CalculateSize);

        return fileSizes + subdirSizes;
    }

    public BlackListForDuplicatesFile OpenBlackListForDuplicatesFile()
    {
        string blackListPath = Path.Combine(FullPath, "bl-duplicates");
        BlackListForDuplicatesFile blackListFile = new(blackListPath);
        blackListFile.Open();
        return blackListFile;
    }

    public IEnumerable<SnapshotPackage> EnumerateSnapshotPackages()
    {
        string snapshotsDirectoryPath = Path.Combine(FullPath, SnapshotsDirectoryName);

        if (!Directory.Exists(snapshotsDirectoryPath))
            return Enumerable.Empty<SnapshotPackage>();

        return Directory.GetFiles(snapshotsDirectoryPath)
            .Select(x => new SnapshotPackage(x))
            .OrderByDescending(x => x.CreationTime);
    }

    public SnapshotPackage CreateSnapshotPackage(in DateTime creationTime)
    {
        string snapshotsDirectoryPath = Path.Combine(FullPath, SnapshotsDirectoryName);
        SnapshotFilePath snapshotFilePath = new(creationTime, snapshotsDirectoryPath);

        return new SnapshotPackage(snapshotFilePath);
    }

    private JPotInfoFile GetInfoFile()
    {
        if (FullPath == null)
            return null;

        return new JPotInfoFile
        {
            RootPath = FullPath
        };
    }
}