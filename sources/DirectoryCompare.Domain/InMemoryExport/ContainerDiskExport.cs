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

namespace DustInTheWind.DirectoryCompare.InMemoryExport
{
    public class ContainerDiskExport : IDiskExport
    {
        private readonly ContainerBuilder containerBuilder;

        public HContainer Container => containerBuilder?.Container;

        public ContainerDiskExport()
        {
            containerBuilder = new ContainerBuilder();
        }

        public void Open(string originalPath)
        {
            containerBuilder.SetOriginalPath(originalPath);
        }

        public void OpenNewDirectory(HDirectory hDirectory)
        {
            containerBuilder.AddAndOpen(hDirectory);
        }

        public void CloseDirectory()
        {
            containerBuilder.CloseDirectory();
        }

        public void Add(HFile hFile)
        {
            containerBuilder.Add(hFile);
        }

        public void Add(HDirectory hDirectory)
        {
            containerBuilder.Add(hDirectory);
        }

        public void Close()
        {
        }
    }
}