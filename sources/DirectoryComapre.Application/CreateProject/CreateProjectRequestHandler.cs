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

using System;
using System.IO;
using System.Linq;
using DustInTheWind.DirectoryCompare.JsonHashesFile;
using DustInTheWind.DirectoryCompare.ProjectModel;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Application.CreateProject
{
    public class CreateProjectRequestHandler : RequestHandler<CreateProjectRequest>
    {
        protected override void Handle(CreateProjectRequest request)
        {
            // Calculate the absolute path.
            //bool isPathRooted = Path.IsPathRooted(request.DirectoryPath);

            //string rootedPath = !isPathRooted
            //    ? request.DirectoryPath
            //    : Path.GetFullPath(request.DirectoryPath);

            string path = request.DirectoryPath;

            // Create directory if it does not exist
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            // Validate the directory is empty
            bool directoryHasItems = Directory.EnumerateFileSystemEntries(path).Any();
            if (directoryHasItems)
                throw new Exception("Directory is not empty.");

            // create the json project file
            Project project = new Project
            {
                Name = request.Name,
                Path = request.DirectoryPath
            };

            ProjectRepository projectRepository = new ProjectRepository();
            projectRepository.Save(project);
        }
    }

    public class ProjectRepository : IProjectRepository
    {
        public void Save(Project project)
        {
            ProjectFile projectFile = ProjectFile.Create(project);
            projectFile.Save();
        }
    }
}