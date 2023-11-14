// DirectoryCompare
// Copyright (C) 2017-2023 Dust in the Wind
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
using DustInTheWind.DirectoryCompare.DataAccess.Transformations;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Domain.PotModel;
using DustInTheWind.DirectoryCompare.Ports.DataAccess;

namespace DustInTheWind.DirectoryCompare.DataAccess;

public class PotRepository : IPotRepository
{
    private readonly Database database;

    public PotRepository(Database database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public async Task<List<Pot>> Get()
    {
        return (await database.GetPotDirectories())
            .Select(x => x.ToPot())
            .ToList();
    }

    public async Task<Pot> Get(string name, bool includeSnapshots)
    {
        PotDirectory potDirectory = (await database.GetPotDirectories())
            .FirstOrDefault(x => x.InfoFile.IsValid && x.InfoFile.Content.Name == name);

        Pot pot = potDirectory?.ToPot();

        if (pot != null && includeSnapshots)
        {
            IEnumerable<Snapshot> snapshots = potDirectory.GetSnapshotFiles()
                .Where(x => x.Open())
                .Select(x => x.Content.ToSnapshot());

            pot.Snapshots.AddRange(snapshots);
        }

        return pot;
    }

    public async Task<bool> Exists(string name)
    {
        IEnumerable<PotDirectory> potDirectories = await database.GetPotDirectories();
        PotDirectory potDirectory = potDirectories
            .FirstOrDefault(x => x.InfoFile.IsValid && x.InfoFile.Content.Name == name);

        return potDirectory != null;
    }

    public Task Add(Pot pot)
    {
        PotDirectory potDirectory = database.NewPotDirectory();

        potDirectory.InfoFile.Content = new JPotInfo
        {
            Name = pot.Name,
            Path = pot.Path,
            Description = pot.Description
        };

        potDirectory.InfoFile.Save();

        return Task.CompletedTask;
    }

    public async Task Delete(string name)
    {
        IEnumerable<PotDirectory> potDirectories = await database.GetPotDirectories();
        PotDirectory potDirectory = potDirectories
            .FirstOrDefault(x => x.InfoFile.IsValid && x.InfoFile.Content.Name == name);

        if (potDirectory != null)
            potDirectory.Delete();
        else
            throw new Exception($"Pot '{name}' does not exist.");
    }
}