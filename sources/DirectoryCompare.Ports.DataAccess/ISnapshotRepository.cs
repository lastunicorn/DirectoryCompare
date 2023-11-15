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

using DustInTheWind.DirectoryCompare.DataStructures;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.Ports.DataAccess.ImportExport;

namespace DustInTheWind.DirectoryCompare.Ports.DataAccess;

public interface ISnapshotRepository
{
    Task<ISnapshotWriter> CreateWriter(string potName);

    IAsyncEnumerable<Snapshot> GetByPot(string potName);

    Task<Snapshot> GetByIndex(string potName, int index);

    Task<Snapshot> GetLast(string potName);

    IAsyncEnumerable<Snapshot> GetByDate(string potName, DateTime dateTime);

    Task<Snapshot> GetByExactDateTime(string potName, DateTime dateTime);

    Task Add(string potName, Snapshot snapshot);

    Task DeleteByIndex(string potName, int index);

    Task DeleteLast(string potName);

    Task<bool> DeleteSingleByDate(string potName, DateTime dateTime);

    Task<bool> DeleteByExactDateTime(string potName, DateTime dateTime);

    Task<Snapshot> Get(SnapshotLocation location);
}