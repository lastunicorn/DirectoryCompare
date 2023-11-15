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
using DustInTheWind.DirectoryCompare.Ports.DataAccess;

namespace DustInTheWind.DirectoryCompare.DataAccess;

public class Database
{
    private DatabaseState state = DatabaseState.Closed;
    private string rootDirectory;

    public void Open(string connectionString)
    {
        if (state == DatabaseState.Opened)
            return;

        rootDirectory = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

        if (!Directory.Exists(rootDirectory))
            Directory.CreateDirectory(rootDirectory);

        if (!Directory.Exists(rootDirectory))
            throw new DatabaseOpenException();

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

    public PotDirectory NewPotDirectory()
    {
        if (state != DatabaseState.Opened)
            throw new DatabaseNotOpenedException();

        PotDirectory potDirectory = PotDirectory.New(rootDirectory);
        potDirectory.Create();
        return potDirectory;
    }
}