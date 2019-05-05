// DirectoryCompare
// Copyright (C) 2017-2019 Dust in the Wind
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

using Newtonsoft.Json;
using System;
using System.IO;
using DirectoryCompare.CliFramework;
using DustInTheWind.DirectoryCompare.JsonHashesFile.Serialization;

namespace DustInTheWind.DirectoryCompare.Cli.Commands
{
    internal class ReadFileCommand : ICommand
    {
        public ProjectLogger Logger { get; set; }
        public string FilePath { get; set; }

        public void DisplayInfo()
        {
            Console.WriteLine("Reading file: " + FilePath);
        }

        public void Initialize(Arguments arguments)
        {
            Logger = new ProjectLogger();
            FilePath = arguments[0];
        }

        public void Execute()
        {

            JsonFileSerializer jsonFileSerializer = new JsonFileSerializer();
            HContainer hContainer = jsonFileSerializer.ReadFromFile(FilePath);

            //string json = File.ReadAllText(FilePath);
            //HContainer hContainer = JsonConvert.DeserializeObject<HContainer>(json);

            //CustomConsole.WriteLine("Container has {0} directories and {1} files.", container.Directories.Count, container.Files.Count);

            ContainerView containerView = new ContainerView(hContainer);
            containerView.Display();
        }
    }
}