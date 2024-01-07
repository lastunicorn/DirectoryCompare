// Directory Compare
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

using Microsoft.Extensions.Configuration;

namespace DustInTheWind.DirectoryCompare.ConfigAccess;

public class ConfigBase
{
    protected readonly IConfiguration Configuration;

    protected ConfigBase(string configFilePath)
    {
        Configuration = new ConfigurationBuilder()
            .AddJsonFile(configFilePath)
            .Build();
    }

    protected bool GetBool(string name)
    {
        string rawValue = Configuration[name];

        if (string.IsNullOrEmpty(rawValue))
            return false;

        return bool.Parse(rawValue);
    }

    protected IEnumerable<string> GetArray(string name)
    {
        IConfigurationSection configurationSection = Configuration.GetSection(name);

        string[] value = configurationSection.Get<string[]>();
        return value ?? Enumerable.Empty<string>();
    }
}