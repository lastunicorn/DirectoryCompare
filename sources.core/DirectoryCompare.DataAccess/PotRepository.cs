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
    public List<Pot> Get()
    {
        return Directory.GetDirectories(Database.Location)
            .Select(x => new PotDirectory(x))
            .Select(ToPot)
            .Where(x => x != null)
            .ToList();
    }

    public Pot Get(string name)
    {
        PotDirectory potDirectory = PotDirectory.FromPotName(name, Database.Location);
        return ToPot(potDirectory);
    }

    private static Pot ToPot(PotDirectory potDirectory)
    {
        if (!potDirectory.IsValid)
            return null;

        JPotInfoFile jPotInfoFile = potDirectory.GetInfoFile();
        bool success = jPotInfoFile.TryOpen();

        if (!success)
            return null;

        return new Pot
        {
            Guid = potDirectory.PotGuid,
            Name = jPotInfoFile.Content.Name,
            Path = jPotInfoFile.Content.Path,
            Description = jPotInfoFile.Content.Description
        };
    }

    public bool Exists(string name)
    {
        PotDirectory potDirectory = PotDirectory.FromPotName(name, Database.Location);
        return potDirectory.IsValid;
    }

    public void Add(Pot pot)
    {
        PotDirectory potDirectory = PotDirectory.FromPotName(pot.Name, Database.Location);
        potDirectory.Create();

        JPotInfoFile jPotInfoFile = potDirectory.GetInfoFile();
        jPotInfoFile.Content = new JPotInfo
        {
            Name = pot.Name,
            Path = pot.Path,
            Description = pot.Description
        };

        jPotInfoFile.Save();
    }

    public void Delete(string name)
    {
        PotDirectory potDirectory = PotDirectory.FromPotName(name, Database.Location);

        if (potDirectory.IsValid)
            potDirectory.Delete();
        else
            throw new Exception($"Pot '{name}' does not exist.");
    }
}