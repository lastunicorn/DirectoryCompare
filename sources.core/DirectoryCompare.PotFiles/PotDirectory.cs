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

using DustInTheWind.DirectoryCompare.JFiles.BlacklistFileModel;
using DustInTheWind.DirectoryCompare.JFiles.PotInfoFileModel;

namespace DustInTheWind.DirectoryCompare.JFiles;

public class PotDirectory
{
    private const string SnapshotsDirectoryName = "snapshots";
    private JPotInfoFile jPotInfoFile;

    public string RootPath { get; private set; }

    public string FullPath { get; private set; }

    public JPotInfoFile InfoFile
    {
        get
        {
            if (jPotInfoFile == null)
            {
                jPotInfoFile = GetInfoFile();
                jPotInfoFile.TryOpen();
            }

            return jPotInfoFile;
        }
    }

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
            
            Guid potGuid = PotGuid;

            if (!InfoFile.IsValid)
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
                bool success = jPotInfoFile.TryOpen();
                return success && jPotInfoFile.Content.Name == potName;
            })
            .FirstOrDefault();

        return potDirectory ?? PotDirectory.New(storagePath);
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
            throw new Exception("The pot directory already exists.");

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

        throw new Exception("Could not find a valid name for the pot's directory. All the tried name already exist.");
    }

    public void Delete()
    {
        Directory.Delete(FullPath, true);
    }

    public BlackListFile OpenBlackListFile(string blackListName)
    {
        string blackListPath = Path.Combine(FullPath, blackListName);
        BlackListFile blackListFile = new BlackListFile(blackListPath);
        blackListFile.Open();
        return blackListFile;
    }

    public IEnumerable<SnapshotFile> GetSnapshotFiles()
    {
        string snapshotsDirectoryPath = Path.Combine(FullPath, SnapshotsDirectoryName);

        if (!Directory.Exists(snapshotsDirectoryPath))
            return Enumerable.Empty<SnapshotFile>();

        return Directory.GetFiles(snapshotsDirectoryPath)
            .Select(x => new SnapshotFile(x))
            .OrderByDescending(x => x.CreationTime);
    }

    public SnapshotFile CreateSnapshotFile(in DateTime creationTime)
    {
        string snapshotsDirectoryPath = Path.Combine(FullPath, SnapshotsDirectoryName);
        SnapshotFilePath snapshotFilePath = new SnapshotFilePath(creationTime, snapshotsDirectoryPath);

        return new SnapshotFile(snapshotFilePath);
    }

    public JPotInfoFile GetInfoFile()
    {
        if (FullPath == null)
            return null;

        string infoFilePath = Path.Combine(FullPath, "info.json");
        return new JPotInfoFile(infoFilePath);
    }
}