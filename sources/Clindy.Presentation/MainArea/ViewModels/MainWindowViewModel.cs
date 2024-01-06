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

using System.Reflection;
using DustInTheWind.Clindy.Applications;
using DustInTheWind.Clindy.Applications.LoadDuplicates;
using DustInTheWind.Clindy.Presentation.DuplicatesNavigatorArea.ViewModels;
using DustInTheWind.Clindy.Presentation.FileGroupDetailsArea.ViewModels;

namespace DustInTheWind.Clindy.Presentation.MainArea.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly RequestBus requestBus;

    public string Title
    {
        get
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            AssemblyName assemblyName = assembly?.GetName();
            Version version = assemblyName?.Version;

            string title = "Clindy";

            if (version != null)
                title += " " + version.ToString(3);

            return title;
        }
    }

    public FileGroupViewModel FileGroupViewModel { get; }

    public DuplicatesNavigatorViewModel DuplicatesNavigatorViewModel { get; }

    public MainWindowViewModel()
    {
    }

    public MainWindowViewModel(RequestBus requestBus,
        DuplicatesNavigatorViewModel duplicatesNavigatorViewModel, FileGroupViewModel fileGroupViewModel)
    {
        this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));

        DuplicatesNavigatorViewModel = duplicatesNavigatorViewModel ?? throw new ArgumentNullException(nameof(duplicatesNavigatorViewModel));
        FileGroupViewModel = fileGroupViewModel ?? throw new ArgumentNullException(nameof(fileGroupViewModel));

        LoadDuplicates();
    }

    private void LoadDuplicates()
    {
        LoadDuplicatesRequest request = new();
        _ = requestBus.PlaceRequest(request);
    }
}