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

using DustInTheWind.DirectoryCompare.JFiles;

namespace DustInTheWind.DirectoryCompare.DataAccess;

public class Database
{
    private string location;

    public IEnumerable<PotDirectory> PotDirectories
    {
        get
        {
            if (location == null)
                throw new Exception("The database is not opened.");

            return Directory.GetDirectories(location)
                .Select(x => new PotDirectory(x))
                .Where(x => x.IsValid);
        }
    }

    public void Open(string connectionString)
    {
        if (!Directory.Exists(connectionString))
            throw new Exception($"Database '{connectionString}' does not exist.");

        location = connectionString;
    }

    public PotDirectory NewPotDirectory()
    {
        if (location == null)
            throw new Exception("The database is not opened.");

        PotDirectory potDirectory = PotDirectory.New(location);
        potDirectory.Create();
        return potDirectory;
    }
}