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

using DustInTheWind.DirectoryCompare.Domain.Utils;
using DustInTheWind.DirectoryCompare.JFiles;
using DustInTheWind.DirectoryCompare.JFiles.BlacklistFileModel;
using DustInTheWind.DirectoryCompare.Ports.DataAccess;

namespace DustInTheWind.DirectoryCompare.DataAccess;

public class BlackListRepository : IBlackListRepository
{
    private readonly Database database;

    public BlackListRepository(Database database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public DiskPathCollection Get(string potName)
    {
        PotDirectory potDirectory = database.PotDirectories
            .FirstOrDefault(x => x.InfoFile.IsValid && x.InfoFile.Content.Name == potName);

        if (potDirectory == null)
            throw new Exception($"There is no pot with name '{potName}'.");

        BlackListFile blackListFile = potDirectory.OpenBlackListFile("bl");
        return new DiskPathCollection(blackListFile.Items);
    }

    public void Add(string potName, DiskPath path)
    {
        PotDirectory potDirectory = database.PotDirectories
            .FirstOrDefault(x => x.InfoFile.IsValid && x.InfoFile.Content.Name == potName);

        if (potDirectory == null)
            throw new Exception($"There is no pot with name '{potName}'.");

        BlackListFile blackListFile = potDirectory.OpenBlackListFile("bl");
        blackListFile.Add(path);
        blackListFile.Save();
    }

    public void Delete(string potName, DiskPath path)
    {
        PotDirectory potDirectory = database.PotDirectories
            .FirstOrDefault(x => x.InfoFile.IsValid && x.InfoFile.Content.Name == potName);

        if (potDirectory == null)
            throw new Exception($"There is no pot with name '{potName}'.");

        BlackListFile blackListFile = potDirectory.OpenBlackListFile("bl");
        blackListFile.Remove(path);
        blackListFile.Save();
    }
}