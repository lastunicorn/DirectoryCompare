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
using DustInTheWind.Clindy.Applications.SetCurrentDuplicateGroup;
using ReactiveUI;

namespace DustInTheWind.Clindy.Presentation.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly RequestBus requestBus;
    private List<DuplicateFilesListItem> duplicateFiles;
    private OpenInExplorerCommand openInExplorerCommand;

    public List<DuplicateFilesListItem> DuplicateFiles
    {
        get => duplicateFiles;
        set => this.RaiseAndSetIfChanged(ref duplicateFiles, value);
    }

    public DuplicatesNavigatorViewModel DuplicatesNavigatorViewModel { get; }

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

    public MainWindowViewModel()
    {
    }

    public MainWindowViewModel(RequestBus requestBus, EventBus eventBus)
    {
        if (eventBus == null) throw new ArgumentNullException(nameof(eventBus));
        this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));

        openInExplorerCommand = new OpenInExplorerCommand();
        DuplicatesNavigatorViewModel = new DuplicatesNavigatorViewModel(requestBus, eventBus);

        eventBus.Subscribe<CurrentDuplicateReplacedEvent>(HandleCurrentDuplicateReplacedEvent);

        LoadDuplicates();
    }

    private Task HandleCurrentDuplicateReplacedEvent(CurrentDuplicateReplacedEvent ev, CancellationToken cancellationToken)
    {
        DuplicateFiles = ev.DuplicateGroup?.FilePaths
            .Select(x => new DuplicateFilesListItem
            {
                FilePath = x,
                OpenCommand = openInExplorerCommand
            })
            .ToList();

        return Task.CompletedTask;
    }

    private async void LoadDuplicates()
    {
        await Task.Delay(500);

        LoadDuplicatesRequest request = new();
        await requestBus.PlaceRequest(request);
    }
}