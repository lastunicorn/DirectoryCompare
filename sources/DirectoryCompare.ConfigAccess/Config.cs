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

using DustInTheWind.DirectoryCompare.Ports.ConfigAccess;
using Microsoft.Extensions.Configuration;

namespace DustInTheWind.DirectoryCompare.ConfigAccess;

public class Config : IConfig
{
    private readonly IConfiguration configuration;

    /// <summary>
    /// Only for CLI
    /// </summary>
    public string ConnectionString => configuration["ConnectionString"];

    /// <summary>
    /// Only for GUI
    /// </summary>
    public string DuplicatesFilePath => configuration["DuplicatesFilePath"];

    /// <summary>
    /// Only for GUI
    /// </summary>
    public bool CheckFilesExistence
    {
        get
        {
            string rawValue = configuration["CheckFilesExistence"];
            return string.IsNullOrEmpty(rawValue)
                ? false
                : bool.Parse(rawValue);
        }
    }

    public Config()
    {
        configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
    }
}