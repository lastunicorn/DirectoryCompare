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

using DustInTheWind.Clindy.Applications;
using DustInTheWind.Clindy.Applications.LoadDuplicates;
using DustInTheWind.Clindy.Applications.SetCurrentDuplicateGroup;
using DustInTheWind.Clindy.Presentation.ViewModels;
using ReactiveUI;

namespace DustInTheWind.Clindy.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly RequestBus requestBus;
    private List<DuplicateFilesListItem> duplicateFiles;

    public List<DuplicateFilesListItem> DuplicateFiles
    {
        get => duplicateFiles;
        set => this.RaiseAndSetIfChanged(ref duplicateFiles, value);
    }

    public DuplicatesNavigatorViewModel DuplicatesNavigatorViewModel { get; }

    public MainWindowViewModel()
    {
    }

    public MainWindowViewModel(RequestBus requestBus, EventBus eventBus)
    {
        if (eventBus == null) throw new ArgumentNullException(nameof(eventBus));
        this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));

        DuplicatesNavigatorViewModel = new DuplicatesNavigatorViewModel(requestBus, eventBus);
        
        eventBus.Subscribe<CurrentDuplicateReplacedEvent>(HandleCurrentDuplicateReplacedEvent);
        
        LoadDuplicates();
    }

    private Task HandleCurrentDuplicateReplacedEvent(CurrentDuplicateReplacedEvent ev, CancellationToken cancellationToken)
    {
        DuplicateFiles = ev.DuplicateGroup.FilePaths
            .Select(x => new DuplicateFilesListItem(x))
            .ToList();

        return Task.CompletedTask;
    }

    private async void LoadDuplicates()
    {
        LoadDuplicatesRequest request = new()
        {
            FilePath = "/home/alez/Temp/amma.json",
            CheckFilesExistence = true
        };
        await requestBus.PlaceRequest(request);
    }
}