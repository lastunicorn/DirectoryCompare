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

    public async Task<IEnumerable<Pot>> GetAll()
    {
        IEnumerable<PotDirectory> potDirectories = await database.GetPotDirectories();
        return potDirectories
            .Select(x => x.ToPot());
    }

    public async Task<Pot> GetByName(string name, bool includeSnapshots)
    {
        IEnumerable<PotDirectory> potDirectories = await database.GetPotDirectories();
        PotDirectory potDirectory = potDirectories
            .FirstOrDefault(x => x.InfoFile.IsValid && x.InfoFile.Content.Name == name);

        Pot pot = potDirectory?.ToPot();

        if (pot != null && includeSnapshots)
            LoadSnapshots(potDirectory, pot);

        return pot;
    }

    public async Task<Pot> GetById(Guid id, bool includeSnapshots)
    {
        IEnumerable<PotDirectory> potDirectories = await database.GetPotDirectories();
        PotDirectory potDirectory = potDirectories
            .FirstOrDefault(x => x.InfoFile.IsValid && x.PotGuid == id);

        Pot pot = potDirectory?.ToPot();

        if (pot != null && includeSnapshots)
            LoadSnapshots(potDirectory, pot);

        return pot;
    }

    public async Task<Pot> GetByPartialId(string partialId, bool includeSnapshots)
    {
        IEnumerable<PotDirectory> potDirectories = await database.GetPotDirectories();
        PotDirectory potDirectory = potDirectories
            .Where(x => x.InfoFile.IsValid)
            .FirstOrDefault(x => x.PotGuid.ToString("N").StartsWith(partialId, StringComparison.InvariantCultureIgnoreCase));

        Pot pot = potDirectory?.ToPot();

        if (pot != null && includeSnapshots)
            LoadSnapshots(potDirectory, pot);

        return pot;
    }

    public async Task<Pot> GetByNameOrId(string nameOrId, bool includeSnapshots = false)
    {
        if (nameOrId == null) throw new ArgumentNullException(nameof(nameOrId));

        if (string.IsNullOrEmpty(nameOrId))
            throw new ArgumentException("The name or id must be provided.", nameof(nameOrId));

        Pot pot = await GetByName(nameOrId, includeSnapshots);

        if (pot != null)
            return pot;

        bool parseSuccess = Guid.TryParse(nameOrId, out Guid guid);

        if (parseSuccess)
            pot = await GetById(guid, includeSnapshots);

        if (pot != null)
            return pot;

        if (nameOrId.Length >= 8)
            pot = await GetByPartialId(nameOrId, includeSnapshots);

        return pot;
    }

    public async Task<bool> ExistsByName(string name)
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

    public async Task<bool> DeleteByName(string name)
    {
        if (name == null) throw new ArgumentNullException(nameof(name));
        if (string.IsNullOrEmpty(name)) throw new ArgumentException("The name was not provided.", nameof(name));

        IEnumerable<PotDirectory> potDirectories = await database.GetPotDirectories();
        PotDirectory potDirectory = potDirectories
            .FirstOrDefault(x => x.InfoFile.IsValid && x.InfoFile.Content.Name == name);

        if (potDirectory == null)
            return false;

        potDirectory.Delete();
        return true;
    }

    public async Task<bool> DeleteById(Guid id)
    {
        IEnumerable<PotDirectory> potDirectories = await database.GetPotDirectories();
        PotDirectory potDirectory = potDirectories
            .FirstOrDefault(x => x.InfoFile.IsValid && x.PotGuid == id);

        if (potDirectory == null)
            return false;

        potDirectory.Delete();
        return true;
    }

    private static void LoadSnapshots(PotDirectory potDirectory, Pot pot)
    {
        IEnumerable<Snapshot> snapshots = potDirectory.EnumerateSnapshotFiles()
            .Where(x => x.Open())
            .Select(x => x.Content.ToSnapshot());

        pot.Snapshots.AddRange(snapshots);
    }
}