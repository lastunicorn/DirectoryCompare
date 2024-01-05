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
using DustInTheWind.Clindy.Applications.PresentDuplicates;
using DustInTheWind.Clindy.Applications.SetCurrentDuplicateGroup;
using ReactiveUI;

namespace DustInTheWind.Clindy.Presentation.ViewModels;

public class DuplicatesNavigatorViewModel : ViewModelBase
{
    private readonly RequestBus requestBus;

    private List<DuplicateGroupListItem> duplicateGroups;
    private DuplicateGroupListItem selectedDuplicateGroup;
    private bool isLoading;
    private int duplicateGroupCount;

    public bool IsLoading
    {
        get => isLoading;
        set => this.RaiseAndSetIfChanged(ref isLoading, value);
    }

    public List<DuplicateGroupListItem> DuplicateGroups
    {
        get => duplicateGroups;
        private set => this.RaiseAndSetIfChanged(ref duplicateGroups, value);
    }

    public DuplicateGroupListItem SelectedDuplicateGroup
    {
        get => selectedDuplicateGroup;
        set
        {
            this.RaiseAndSetIfChanged(ref selectedDuplicateGroup, value);
            SetCurrentDuplicateGroup(value);
        }
    }

    private void SetCurrentDuplicateGroup(DuplicateGroupListItem value)
    {
        SetCurrentDuplicateGroupRequest request = new()
        {
            Hash = value.DuplicateGroup.FileHash
        };
        _ = requestBus.PlaceRequest(request);
    }

    public int DuplicateGroupCount
    {
        get => duplicateGroupCount;
        set => this.RaiseAndSetIfChanged(ref duplicateGroupCount, value);
    }

    public DuplicatesNavigatorViewModel()
    {
    }

    public DuplicatesNavigatorViewModel(RequestBus requestBus, EventBus eventBus)
    {
        if (eventBus == null) throw new ArgumentNullException(nameof(eventBus));
        this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));

        eventBus.Subscribe<DuplicatesListChangedEvent>(HandleDuplicatesListChangedEvent);
    }

    private Task HandleDuplicatesListChangedEvent(DuplicatesListChangedEvent ev, CancellationToken cancellationToken)
    {
        return LoadDuplicates();
    }

    private async Task LoadDuplicates()
    {
        IsLoading = true;
        try
        {
            PresentDuplicatesRequest request = new();
            PresentDuplicatesResponse response = await requestBus.PlaceRequest<PresentDuplicatesRequest, PresentDuplicatesResponse>(request);

            DuplicateGroups = response.Duplicates
                .Select(x => new DuplicateGroupListItem(x))
                .OrderByDescending(x => x.FileSize)
                .ToList();

            DuplicateGroupCount = DuplicateGroups.Count;
        }
        finally
        {
            IsLoading = false;
        }
    }
}