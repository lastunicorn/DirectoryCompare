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

using DustInTheWind.DirectoryCompare.Cli.UI;
using DustInTheWind.DirectoryCompare.DataAccess;
using DustInTheWind.DirectoryCompare.DiskAnalysis;
using DustInTheWind.DirectoryCompare.Domain.DataAccess;
using DustInTheWind.DirectoryCompare.Domain.DiskAnalysis;
using DustInTheWind.DirectoryCompare.Domain.Logging;
using Ninject;

namespace DustInTheWind.DirectoryCompare.Cli.Setup
{
    internal static class DependencyContainerSetup
    {
        public static KernelBase Setup()
        {
            StandardKernel kernel = new StandardKernel();

            kernel.Bind<IProjectLogger>().To<ProjectLogger>().InSingletonScope();
            kernel.Bind<IProjectRepository>().To<ProjectRepository>();
            kernel.Bind<IPotRepository>().To<PotRepository>();
            kernel.Bind<IBlackListRepository>().To<BlackListRepository>();
            kernel.Bind<ISnapshotRepository>().To<SnapshotRepository>();
            kernel.Bind<IAnalysisExportFactory>().To<AnalysisExportFactory>();
            kernel.Bind<IDiskAnalyzerFactory>().To<DiskAnalyzerFactory>().InSingletonScope();

            return kernel;
        }
    }
}