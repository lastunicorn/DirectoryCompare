// Directory Compare
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

using DustInTheWind.DirectoryCompare.DataAccess.PotFiles;
using DustInTheWind.DirectoryCompare.DataAccess.PotFiles.PotInfoFileModel;
using DustInTheWind.DirectoryCompare.Ports.DataAccess;

namespace DustInTheWind.DirectoryCompare.DataAccess;

public class Database
{
    private DatabaseState state = DatabaseState.Closed;
    private string rootDirectory;

    public Database(string connectionString)
    {
        rootDirectory = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public bool Exists()
    {
        if (!Directory.Exists(rootDirectory))
            return false;

        JDatabaseInfoFile jDatabaseInfoFile = new()
        {
            RootPath = rootDirectory
        };

        if (!jDatabaseInfoFile.Exists)
            return false;

        JDatabaseInfo jDatabaseInfo = jDatabaseInfoFile.Read();

        if (jDatabaseInfo.Version != "1.0")
            return false;

        return true;
    }

    public void Create()
    {
        if (Directory.Exists(rootDirectory))
        {
            bool rootDirectoryHasChildren = Directory.EnumerateFileSystemEntries(rootDirectory).Any();
            if (rootDirectoryHasChildren)
                throw new DatabaseCreateException(rootDirectory);
        }
        else
        {
            Directory.CreateDirectory(rootDirectory);
        }

        if (!Directory.Exists(rootDirectory))
            throw new DatabaseCreateException(rootDirectory);

        JDatabaseInfoFile jDatabaseInfoFile = new()
        {
            RootPath = rootDirectory
        };

        if (!jDatabaseInfoFile.Exists)
        {
            JDatabaseInfo jDatabaseInfo = new()
            {
                Version = "1.0"
            };
            jDatabaseInfoFile.SaveNew(jDatabaseInfo);
        }
    }

    public void Open()
    {
        if (state == DatabaseState.Opened)
            return;

        if (!Exists())
            throw new DatabaseOpenException(rootDirectory);

        state = DatabaseState.Opened;
    }

    public async Task<IEnumerable<PotDirectory>> GetPotDirectories()
    {
        if (state != DatabaseState.Opened)
            throw new DatabaseNotOpenedException();

        return await Task.Run(() =>
        {
            return Directory.GetDirectories(rootDirectory)
                .Select(x => new PotDirectory(x))
                .Where(x => x.IsValid);
        });
    }

    public async Task<PotDirectory> GetPotDirectory(string potName)
    {
        IEnumerable<PotDirectory> potDirectories = await GetPotDirectories();

        return potDirectories
            .FirstOrDefault(x =>
            {
                JPotInfo jPotInfo = x.InfoFile.Read();
                return jPotInfo != null && jPotInfo.Name == potName;
            });
    }

    public async Task<PotDirectory> GetPotDirectory(Guid id)
    {
        IEnumerable<PotDirectory> potDirectories = await GetPotDirectories();

        return potDirectories
            .FirstOrDefault(x => x.InfoFile.IsValid && x.PotGuid == id);
    }

    public PotDirectory NewPotDirectory()
    {
        if (state != DatabaseState.Opened)
            throw new DatabaseNotOpenedException();

        PotDirectory potDirectory = PotDirectory.New(rootDirectory);
        potDirectory.Create();
        return potDirectory;
    }
}