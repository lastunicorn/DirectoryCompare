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

using DustInTheWind.DirectoryCompare.Domain.DataAccess;
using DustInTheWind.DirectoryCompare.Domain.Entities;
using DustInTheWind.DirectoryCompare.JsonHashesFile;
using DustInTheWind.DirectoryCompare.JsonHashesFile.Serialization;

namespace DustInTheWind.DirectoryCompare.DataAccess
{
    public class ProjectRepository : IProjectRepository
    {
        public void Save(Project project)
        {
            ProjectFile projectFile = new ProjectFile
            {
                Name = project.Name,
                Path = project.Path + "project.json"
            };

            projectFile.Save();
        }

        public Snapshot GetSnapshot(string path)
        {
            JsonSnapshotFile file = JsonSnapshotFile.Load(path);
            return file.Snapshot;
        }
    }
}