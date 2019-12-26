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

using DustInTheWind.DirectoryCompare.Application;
using DustInTheWind.DirectoryCompare.Application.UseCases.ComparePaths;
using DustInTheWind.DirectoryCompare.Application.UseCases.CompareSnapshots;
using DustInTheWind.DirectoryCompare.Application.UseCases.CreateProject;
using DustInTheWind.DirectoryCompare.Application.UseCases.CreateSnapshot;
using DustInTheWind.DirectoryCompare.Application.UseCases.FindDuplicates;
using DustInTheWind.DirectoryCompare.Application.UseCases.GetSnapshot;
using DustInTheWind.DirectoryCompare.Application.UseCases.RemoveDuplicates;
using DustInTheWind.DirectoryCompare.Application.UseCases.VerifyDisk;

namespace DustInTheWind.DirectoryCompare.Cli
{
    internal class MyRequestBus : RequestBus
    {
        public MyRequestBus()
        {
            Register(typeof(ComparePathsRequest), typeof(ComparePathsRequestHandler));
            Register(typeof(CompareSnapshotsRequest), typeof(CompareSnapshotsRequestHandler));
            Register(typeof(CreateProjectRequest), typeof(CreateProjectRequestHandler));
            Register(typeof(CreateSnapshotRequest), typeof(CreateSnapshotRequestHandler));
            Register(typeof(FindDuplicatesRequest), typeof(FindDuplicatesRequestHandler));
            Register(typeof(GetSnapshotRequest), typeof(GetSnapshotRequestHandler));
            Register(typeof(RemoveDuplicatesRequest), typeof(RemoveDuplicatesRequestHandler));
            Register(typeof(VerifyDiskRequest), typeof(VerifyDiskRequestHandler));
        }
    }
}