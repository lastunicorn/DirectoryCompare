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

        SnapshotFile snapshotFile = await GetSnapshotFile(location);

        if (snapshotFile == null)
            return null;

        snapshotFile.Open();
        return snapshotFile.Document.ToSnapshot();
    }

    public async IAsyncEnumerable<Snapshot> GetByPot(string potName)
    {
        PotDirectory potDirectory = await database.GetPotDirectory(potName);

        if (potDirectory == null)
            throw new PotNotFoundException(potName);

        IEnumerable<SnapshotFile> allSnapshotFiles = potDirectory.EnumerateSnapshotFiles();

        foreach (SnapshotFile snapshotFile in allSnapshotFiles)
        {
            bool success = snapshotFile.Open();

            if (success)
                yield return snapshotFile.Document.ToSnapshot();
        }
    }

    public async Task<ISnapshotWriter> CreateWriter(string potName)
    {
        PotDirectory potDirectory = await database.GetPotDirectory(potName);

        if (potDirectory == null)
            throw new Exception($"There is no pot with name '{potName}'.");

        SnapshotFile snapshotFile = potDirectory.CreateSnapshotFile(DateTime.UtcNow);
        JSnapshotWriter jSnapshotWriter = snapshotFile.OpenSnapshotWriter();
        return new JsonSnapshotWriter(jSnapshotWriter);
    }

    public async Task Add(string potName, Snapshot snapshot)
    {
        PotDirectory potDirectory = await database.GetPotDirectory(potName);

        if (potDirectory == null)
            throw new Exception($"There is no pot with name '{potName}'.");

        SnapshotFile snapshotFile = potDirectory.CreateSnapshotFile(snapshot.CreationTime);
        snapshotFile.Open();
        snapshotFile.Document = snapshot.ToJSnapshot();
        snapshotFile.Save();
    }

    public async Task DeleteByIndex(string potName, int index = 0)
    {
        PotDirectory potDirectory = await database.GetPotDirectory(potName);

        if (potDirectory == null)
            throw new Exception($"There is no pot with name '{potName}'.");

        SnapshotFile snapshotFile = potDirectory.EnumerateSnapshotFiles()
            .Skip(index)
            .FirstOrDefault();

        snapshotFile?.Delete();
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

        SnapshotFile[] snapshotFiles = potDirectory.EnumerateSnapshotFiles()
            .Where(x => x.CreationTime.HasValue && x.CreationTime.Value.Date == dateTime.Date)
            .ToArray();

        if (snapshotFiles.Length == 0)
            return false;

        if (snapshotFiles.Length > 1)
            throw new Exception($"There are multiple snapshots that match the specified date. Pot = {potName}; Date = {dateTime}");

        snapshotFiles.First().Delete();
        return true;
    }

    public async Task<bool> DeleteByExactDateTime(string potName, DateTime dateTime)
    {
        PotDirectory potDirectory = await database.GetPotDirectory(potName);

        if (potDirectory == null)
            throw new Exception($"There is no pot with name '{potName}'.");

        SnapshotFile snapshotFile = potDirectory.EnumerateSnapshotFiles()
            .FirstOrDefault(x => x.CreationTime.HasValue && x.CreationTime.Value == dateTime);

        if (snapshotFile == null)
            return false;

        snapshotFile.Delete();
        return true;
    }

    public async Task<DataSize> GetStorageSize(SnapshotLocation location)
    {
        if (string.IsNullOrEmpty(location.PotName))
            return DataSize.Zero;

        SnapshotFile snapshotFile = await GetSnapshotFile(location);

        return snapshotFile?.Size ?? DataSize.Zero;
    }

    public async Task<DataSize> GetSnapshotSize(Guid potId, Guid snapshotId)
    {
        SnapshotFile snapshotFile = await GetById(potId, snapshotId);
        return snapshotFile?.Size ?? DataSize.Zero;
    }

    private async Task<SnapshotFile> GetById(Guid potId, Guid snapshotId)
    {
        PotDirectory potDirectory = await database.GetPotDirectory(potId);

        if (potDirectory == null)
            return null;

        IEnumerable<SnapshotFile> snapshotFiles = potDirectory.EnumerateSnapshotFiles();

        foreach (SnapshotFile snapshotFile in snapshotFiles)
        {
            snapshotFile.Open();

            if (snapshotFile.Document.AnalysisId == snapshotId)
                return snapshotFile;
        }

        return null;
    }

    private async Task<SnapshotFile> GetSnapshotFile(SnapshotLocation location)
    {
        if (location.SnapshotIndex.HasValue)
            return await GetByIndex(location.PotName, location.SnapshotIndex.Value);

        if (location.SnapshotDate.HasValue)
            return await GetByDateAndOptionalTime(location.PotName, location.SnapshotDate.Value);

        return await GetByIndex(location.PotName);
    }

    private async Task<SnapshotFile> GetByIndex(string potName, int index = 0)
    {
        PotDirectory potDirectory = await database.GetPotDirectory(potName);

        return potDirectory?.EnumerateSnapshotFiles()
            .Skip(index)
            .FirstOrDefault();
    }

    private async Task<SnapshotFile> GetByDateAndOptionalTime(string potName, DateTime dateTime)
    {
        SnapshotFile snapshotFile = await GetByExactDateTime(potName, dateTime);

        if (snapshotFile == null && dateTime.TimeOfDay == TimeSpan.Zero)
            snapshotFile = await GetByDateOnly(potName, dateTime);
        
        return snapshotFile;
    }

    private async Task<SnapshotFile> GetByExactDateTime(string potName, DateTime dateTime)
    {
        PotDirectory potDirectory = await database.GetPotDirectory(potName);

        return potDirectory?.EnumerateSnapshotFiles()
            .FirstOrDefault(x => x.CreationTime.HasValue && x.CreationTime.Value == dateTime);
    }

    private async Task<SnapshotFile> GetByDateOnly(string potName, DateTime dateTime)
    {
        PotDirectory potDirectory = await database.GetPotDirectory(potName);

        if (potDirectory == null)
            return null;

        IEnumerable<SnapshotFile> snapshotFiles = potDirectory.EnumerateSnapshotFiles()
            .Where(x => x.CreationTime.HasValue && x.CreationTime.Value.Date == dateTime.Date);

        SnapshotFile theSnapshotFile = null;
        int count = 0;

        foreach (SnapshotFile snapshotFile in snapshotFiles)
        {
            if (count > 0)
                throw new Exception($"There are multiple snapshots that match the specified date. Pot = {potName}; Date = {dateTime}");

            theSnapshotFile = snapshotFile;
            count++;
        }

        return theSnapshotFile;
    }
}