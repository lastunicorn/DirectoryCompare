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

using DustInTheWind.DirectoryCompare.Domain.PotModel;
using DustInTheWind.DirectoryCompare.JFiles;
using DustInTheWind.DirectoryCompare.JFiles.PotInfoFileModel;
using DustInTheWind.DirectoryCompare.Ports.DataAccess;

namespace DustInTheWind.DirectoryCompare.DataAccess;

public class PotRepository : IPotRepository
{
    private readonly Database database;

    public PotRepository(Database database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public List<Pot> Get()
    {
        return database.PotDirectories
            .Select(x => x.ToPot())
            .ToList();
    }

    public Pot Get(string name)
    {
        PotDirectory potDirectory = database.PotDirectories
            .FirstOrDefault(x => x.InfoFile.IsValid && x.InfoFile.Content.Name == name);

        return potDirectory?.ToPot();
    }

    public bool Exists(string name)
    {
        PotDirectory potDirectory = database.PotDirectories
            .FirstOrDefault(x => x.InfoFile.IsValid && x.InfoFile.Content.Name == name);

        return potDirectory != null;
    }

    public void Add(Pot pot)
    {
        PotDirectory potDirectory = database.NewPotDirectory();

        potDirectory.InfoFile.Content = new JPotInfo
        {
            Name = pot.Name,
            Path = pot.Path,
            Description = pot.Description
        };

        potDirectory.InfoFile.Save();
    }

    public void Delete(string name)
    {
        PotDirectory potDirectory = database.PotDirectories
            .FirstOrDefault(x => x.InfoFile.IsValid && x.InfoFile.Content.Name == name);

        if (potDirectory != null)
            potDirectory.Delete();
        else
            throw new Exception($"Pot '{name}' does not exist.");
    }
}