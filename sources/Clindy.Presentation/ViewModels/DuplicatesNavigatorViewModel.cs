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
using DustInTheWind.DirectoryCompare.DataStructures;
using ReactiveUI;

namespace DustInTheWind.Clindy.Presentation.ViewModels;

public class DuplicatesNavigatorViewModel : ViewModelBase
{
    private readonly RequestBus requestBus;

    private List<DuplicateGroupListItem> duplicateGroups;
    private DuplicateGroupListItem selectedDuplicateGroup;
    private bool isLoading;
    private int duplicateGroupCount;
    private string totalSize;

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

    public int DuplicateGroupCount
    {
        get => duplicateGroupCount;
        set => this.RaiseAndSetIfChanged(ref duplicateGroupCount, value);
    }

    public string TotalSize
    {
        get => totalSize;
        set => this.RaiseAndSetIfChanged(ref totalSize, value);
    }

    public RefreshCommand RefreshCommand { get; }

    public DuplicatesNavigatorViewModel()
    {
    }

    public DuplicatesNavigatorViewModel(RequestBus requestBus, EventBus eventBus)
    {
        if (eventBus == null) throw new ArgumentNullException(nameof(eventBus));
        this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));

        RefreshCommand = new RefreshCommand(requestBus);

        eventBus.Subscribe<DuplicatesListLoadingEvent>(HandleDuplicatesListLoadingEvent);
        eventBus.Subscribe<DuplicatesListLoadedEvent>(HandleDuplicatesListLoadedEvent);
    }

    private async Task HandleDuplicatesListLoadingEvent(DuplicatesListLoadingEvent ev, CancellationToken cancellationToken)
    {
        IsLoading = true;
        DuplicateGroups = null;
        DuplicateGroupCount = 0;
        TotalSize = DataSize.Zero.ToString("simple");

        await Task.Delay(500, cancellationToken);
    }

    private Task HandleDuplicatesListLoadedEvent(DuplicatesListLoadedEvent ev, CancellationToken cancellationToken)
    {
        try
        {
            return RetrieveDuplicates();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task RetrieveDuplicates()
    {
        PresentDuplicatesRequest request = new();
        PresentDuplicatesResponse response = await requestBus.PlaceRequest<PresentDuplicatesRequest, PresentDuplicatesResponse>(request);

        DuplicateGroups = response.Duplicates
            .Select(x => new DuplicateGroupListItem(x))
            .OrderByDescending(x => x.FileSize)
            .ToList();

        SelectedDuplicateGroup = IdentifyDuplicateGroup(response.CurrentDuplicateGroup);

        DuplicateGroupCount = DuplicateGroups.Count;
        TotalSize = response.TotalSize.ToString("detailed");
    }

    private DuplicateGroupListItem IdentifyDuplicateGroup(FileGroup? fileGroup)
    {
        return fileGroup == null
            ? null
            : DuplicateGroups.FirstOrDefault(x => x.DuplicateGroup.FileHash == fileGroup.Value.FileHash);
    }

    private void SetCurrentDuplicateGroup(DuplicateGroupListItem value)
    {
        SetCurrentDuplicateGroupRequest request = new()
        {
            Hash = value?.DuplicateGroup.FileHash
        };
        _ = requestBus.PlaceRequest(request);
    }
}