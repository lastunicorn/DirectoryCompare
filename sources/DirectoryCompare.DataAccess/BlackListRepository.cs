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

using DustInTheWind.DirectoryCompare.DataAccess.PotFiles;
using DustInTheWind.DirectoryCompare.DataAccess.PotFiles.BlacklistFileModel;
using DustInTheWind.DirectoryCompare.DataAccess.PotFiles.PotInfoFileModel;
using DustInTheWind.DirectoryCompare.DataStructures;
using DustInTheWind.DirectoryCompare.Ports.DataAccess;

namespace DustInTheWind.DirectoryCompare.DataAccess;

public class BlackListRepository : IBlackListRepository
{
    private readonly Database database;

    public BlackListRepository(Database database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public async Task<DiskPathCollection> Get(string potName)
    {
        PotDirectory potDirectory = await GetPotDirectory(potName);

        JPotInfoFile infoFile = potDirectory.InfoFile;
        JPotInfo jPotInfo = infoFile.Read();

        List<string> excludes = jPotInfo.Exclude ?? new List<string>();
        return new DiskPathCollection(excludes);
    }

    public async Task Add(string potName, DiskPath path)
    {
        PotDirectory potDirectory = await GetPotDirectory(potName);

        JPotInfoFile infoFile = potDirectory.InfoFile;
        JPotInfo jPotInfo = infoFile.Read();

        jPotInfo.Exclude ??= new List<string>();
        jPotInfo.Exclude.Add(path);
        infoFile.SaveChanges();
    }

    public async Task Delete(string potName, DiskPath path)
    {
        PotDirectory potDirectory = await GetPotDirectory(potName);

        JPotInfoFile infoFile = potDirectory.InfoFile;
        JPotInfo jPotInfo = infoFile.Read();

        jPotInfo.Exclude ??= new List<string>();
        jPotInfo.Exclude.Remove(path);
        infoFile.SaveChanges();
    }

    public async Task<FileHashCollection> GetDuplicateExcludes(string potName)
    {
        PotDirectory potDirectory = await GetPotDirectory(potName);

        BlackListForDuplicatesFile blackListFile = potDirectory.OpenBlackListForDuplicatesFile();
        return new FileHashCollection(blackListFile.Items);
    }

    private async Task<PotDirectory> GetPotDirectory(string potName)
    {
        IEnumerable<PotDirectory> potDirectories = await database.GetPotDirectories();
        PotDirectory potDirectory = potDirectories
            .FirstOrDefault(x =>
            {
                JPotInfo jPotInfo = x.InfoFile.Read();
                return jPotInfo != null && jPotInfo.Name == potName;
            });

        if (potDirectory == null)
            throw new Exception($"There is no pot with name '{potName}'.");

        return potDirectory;
    }
}