// VeloCity
// Copyright (C) 2022-2023 Dust in the Wind
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
using DustInTheWind.DirectoryCompare.Ports.DataAccess;

namespace DustInTheWind.DirectoryCompare.Cli.Application.Utils;

internal static class PotRepositoryExtensions
{
    public static async Task<Pot> GetByNameOrId(this IPotRepository potRepository, string nameOrId, bool includeSnapshots = false)
    {
        if (nameOrId == null) throw new ArgumentNullException(nameof(nameOrId));

        if (string.IsNullOrEmpty(nameOrId))
            throw new ArgumentException("The name or id must be provided.", nameof(nameOrId));

        Pot pot = await potRepository.GetByName(nameOrId, includeSnapshots);

        if (pot == null)
        {
            bool parseSuccess = Guid.TryParse(nameOrId, out Guid guid);

            if (parseSuccess)
                pot = await potRepository.GetById(guid, includeSnapshots);
        }

        if (pot == null)
        {
            if (nameOrId.Length >= 8)
                pot = await potRepository.GetByPartialId(nameOrId, includeSnapshots);
        }

        if (pot == null)
            throw new Exception($"Pot '{nameOrId}' does not exist.");

        return pot;
    }
}