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
using DustInTheWind.DirectoryCompare.DataAccess.PotFiles.SnapshotFileModel;
using DustInTheWind.DirectoryCompare.DataAccess.Transformations;
using DustInTheWind.DirectoryCompare.DataStructures;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Domain.PotModel;
using DustInTheWind.DirectoryCompare.Ports.DataAccess;
using DustInTheWind.DirectoryCompare.Ports.DataAccess.ImportExport;

namespace DustInTheWind.DirectoryCompare.DataAccess;

public class SnapshotRepository : ISnapshotRepository
{
    private readonly Database database;

    public SnapshotRepository(Database database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public async Task<Snapshot> Get(SnapshotLocation location)
    {
        if (string.IsNullOrEmpty(location.PotName))
            return null;

        SnapshotPackage snapshotPackage = await GetSnapshotFile(location);

        if (snapshotPackage == null)
            return null;

        snapshotPackage.Open();
        return snapshotPackage.SnapshotContent.ToSnapshot();
    }

    public async IAsyncEnumerable<Snapshot> GetByPot(string potName)
    {
        PotDirectory potDirectory = await database.GetPotDirectory(potName);

        if (potDirectory == null)
            throw new PotNotFoundException(potName);

        IEnumerable<SnapshotPackage> allSnapshotPackages = potDirectory.EnumerateSnapshotPackages();

        foreach (SnapshotPackage snapshotPackage in allSnapshotPackages)
        {
            bool success = snapshotPackage.Open();

            if (success)
                yield return snapshotPackage.SnapshotContent.ToSnapshot();
        }
    }

    public async Task<ISnapshotWriter> CreateWriter(string potName)
    {
        PotDirectory potDirectory = await database.GetPotDirectory(potName);

        if (potDirectory == null)
            throw new Exception($"There is no pot with name '{potName}'.");

        SnapshotPackage snapshotPackage = potDirectory.CreateSnapshotPackage(DateTime.UtcNow);
        JSnapshotWriter jSnapshotWriter = snapshotPackage.OpenSnapshotWriter();
        return new JsonSnapshotWriter(jSnapshotWriter);
    }

    public async Task Add(string potName, Snapshot snapshot)
    {
        PotDirectory potDirectory = await database.GetPotDirectory(potName);

        if (potDirectory == null)
            throw new Exception($"There is no pot with name '{potName}'.");

        SnapshotPackage snapshotPackage = potDirectory.CreateSnapshotPackage(snapshot.CreationTime);
        snapshotPackage.Open();
        snapshotPackage.SnapshotContent = snapshot.ToJSnapshot();
        snapshotPackage.Save();
    }

    public async Task DeleteByIndex(string potName, int index = 0)
    {
        PotDirectory potDirectory = await database.GetPotDirectory(potName);

        if (potDirectory == null)
            throw new Exception($"There is no pot with name '{potName}'.");

        SnapshotPackage snapshotPackage = potDirectory.EnumerateSnapshotPackages()
            .Skip(index)
            .FirstOrDefault();

        snapshotPackage?.Delete();
    }

    public async Task DeleteLast(string potName)
    {
        await DeleteByIndex(potName);
    }

    public async Task<bool> DeleteSingleByDate(string potName, DateTime dateTime)
    {
        PotDirectory potDirectory = await database.GetPotDirectory(potName);

        if (potDirectory == null)
            throw new Exception($"There is no pot with name '{potName}'.");

        SnapshotPackage[] snapshotPackages = potDirectory.EnumerateSnapshotPackages()
            .Where(x => x.CreationTime.HasValue && x.CreationTime.Value.Date == dateTime.Date)
            .ToArray();

        if (snapshotPackages.Length == 0)
            return false;

        if (snapshotPackages.Length > 1)
            throw new Exception($"There are multiple snapshots that match the specified date. Pot = {potName}; Date = {dateTime}");

        snapshotPackages.First().Delete();
        return true;
    }

    public async Task<bool> DeleteByExactDateTime(string potName, DateTime dateTime)
    {
        PotDirectory potDirectory = await database.GetPotDirectory(potName);

        if (potDirectory == null)
            throw new Exception($"There is no pot with name '{potName}'.");

        SnapshotPackage snapshotPackage = potDirectory.EnumerateSnapshotPackages()
            .FirstOrDefault(x => x.CreationTime.HasValue && x.CreationTime.Value == dateTime);

        if (snapshotPackage == null)
            return false;

        snapshotPackage.Delete();
        return true;
    }

    public async Task<DataSize> GetStorageSize(SnapshotLocation location)
    {
        if (string.IsNullOrEmpty(location.PotName))
            return DataSize.Zero;

        SnapshotPackage snapshotPackage = await GetSnapshotFile(location);

        return snapshotPackage?.Size ?? DataSize.Zero;
    }

    public async Task<DataSize> GetSnapshotSize(Guid potId, Guid snapshotId)
    {
        SnapshotPackage snapshotPackage = await GetById(potId, snapshotId);
        return snapshotPackage?.Size ?? DataSize.Zero;
    }

    private async Task<SnapshotPackage> GetById(Guid potId, Guid snapshotId)
    {
        PotDirectory potDirectory = await database.GetPotDirectory(potId);

        if (potDirectory == null)
            return null;

        IEnumerable<SnapshotPackage> snapshotPackages = potDirectory.EnumerateSnapshotPackages();

        foreach (SnapshotPackage snapshotPackage in snapshotPackages)
        {
            snapshotPackage.Open();

            if (snapshotPackage.SnapshotContent.AnalysisId == snapshotId)
                return snapshotPackage;
        }

        return null;
    }

    private async Task<SnapshotPackage> GetSnapshotFile(SnapshotLocation location)
    {
        if (location.SnapshotIndex.HasValue)
            return await GetByIndex(location.PotName, location.SnapshotIndex.Value);

        if (location.SnapshotDate.HasValue)
            return await GetByDateAndOptionalTime(location.PotName, location.SnapshotDate.Value);

        return await GetByIndex(location.PotName);
    }

    private async Task<SnapshotPackage> GetSnapshotPackage(SnapshotLocation location)
    {
        if (location.SnapshotIndex.HasValue)
            return await GetByIndex(location.PotName, location.SnapshotIndex.Value);

        if (location.SnapshotDate.HasValue)
            return await GetByDateAndOptionalTime(location.PotName, location.SnapshotDate.Value);

        return await GetByIndex(location.PotName);
    }

    private async Task<SnapshotPackage> GetByIndex(string potName, int index = 0)
    {
        PotDirectory potDirectory = await database.GetPotDirectory(potName);

        return potDirectory?.EnumerateSnapshotPackages()
            .Skip(index)
            .FirstOrDefault();
    }

    private async Task<SnapshotPackage> GetByDateAndOptionalTime(string potName, DateTime dateTime)
    {
        SnapshotPackage snapshotPackage = await GetByExactDateTime(potName, dateTime);

        if (snapshotPackage == null && dateTime.TimeOfDay == TimeSpan.Zero)
            snapshotPackage = await GetByDateOnly(potName, dateTime);

        return snapshotPackage;
    }

    private async Task<SnapshotPackage> GetByExactDateTime(string potName, DateTime dateTime)
    {
        PotDirectory potDirectory = await database.GetPotDirectory(potName);

        return potDirectory?.EnumerateSnapshotPackages()
            .FirstOrDefault(x => x.CreationTime.HasValue && x.CreationTime.Value == dateTime);
    }

    private async Task<SnapshotPackage> GetByDateOnly(string potName, DateTime dateTime)
    {
        PotDirectory potDirectory = await database.GetPotDirectory(potName);

        if (potDirectory == null)
            return null;

        IEnumerable<SnapshotPackage> snapshotPackages = potDirectory.EnumerateSnapshotPackages()
            .Where(x => x.CreationTime.HasValue && x.CreationTime.Value.Date == dateTime.Date);

        SnapshotPackage theSnapshotPackage = null;
        int count = 0;

        foreach (SnapshotPackage snapshotPackage in snapshotPackages)
        {
            if (count > 0)
                throw new Exception($"There are multiple snapshots that match the specified date. Pot = {potName}; Date = {dateTime}");

            theSnapshotPackage = snapshotPackage;
            count++;
        }

        return theSnapshotPackage;
    }
}