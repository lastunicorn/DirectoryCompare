// DirectoryCompare
// Copyright (C) 2017-2024 Dust in the Wind
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

using System.Collections.ObjectModel;
using Avalonia.Threading;
using DustInTheWind.Clindy.Applications;
using DustInTheWind.Clindy.Applications.LoadDuplicates;
using DustInTheWind.Clindy.Applications.PresentDuplicates;
using DustInTheWind.Clindy.Applications.SetCurrentDuplicateGroup;
using DustInTheWind.DirectoryCompare.Infrastructure;
using DynamicData;
using ReactiveUI;

namespace DustInTheWind.Clindy.Presentation.DuplicatesNavigatorArea.ViewModels;

public class DuplicatesNavigatorViewModel : ViewModelBase
{
    private readonly RequestBus requestBus;

    private DuplicateGroupListItem selectedDuplicateGroup;
    private bool isLoading;

    public bool IsLoading
    {
        get => isLoading;
        set => this.RaiseAndSetIfChanged(ref isLoading, value);
    }

    public ObservableCollection<DuplicateGroupListItem> DuplicateGroups { get; } = new();

    public DuplicateGroupListItem SelectedDuplicateGroup
    {
        get => selectedDuplicateGroup;
        set
        {
            this.RaiseAndSetIfChanged(ref selectedDuplicateGroup, value);

            if (!InitializeMode)
                SetCurrentDuplicateGroup(value);
        }
    }

    public DuplicatesNavigatorHeaderViewModel HeaderViewModel { get; }

    public DuplicatesNavigatorFooterViewModel FooterViewModel { get; }

    public DuplicatesNavigatorViewModel()
    {
    }

    public DuplicatesNavigatorViewModel(RequestBus requestBus, EventBus eventBus,
        DuplicatesNavigatorFooterViewModel footerViewModel, DuplicatesNavigatorHeaderViewModel headerViewModel)
    {
        if (eventBus == null) throw new ArgumentNullException(nameof(eventBus));
        this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));

        FooterViewModel = footerViewModel ?? throw new ArgumentNullException(nameof(footerViewModel));
        HeaderViewModel = headerViewModel ?? throw new ArgumentNullException(nameof(headerViewModel));

        eventBus.Subscribe<DuplicatesLoadingEvent>(HandleDuplicatesLoadingEvent);
        eventBus.Subscribe<DuplicatesLoadedEvent>(HandleDuplicatesLoadedEvent);
    }

    private void HandleDuplicatesLoadingEvent(DuplicatesLoadingEvent ev)
    {
        Dispatcher.UIThread.Post(() =>
        {
            IsLoading = true;

            RunAsInitialize(() =>
            {
                DuplicateGroups.Clear();
                FooterViewModel.Clear();
            });
        });
    }

    private void HandleDuplicatesLoadedEvent(DuplicatesLoadedEvent ev)
    {
        try
        {
            _ = RetrieveDuplicates();
        }
        finally
        {
            Dispatcher.UIThread.Post(() =>
            {
                IsLoading = false;
            });
        }
    }

    private async Task RetrieveDuplicates()
    {
        PresentDuplicatesRequest request = new();
        PresentDuplicatesResponse response = await requestBus.PlaceRequest<PresentDuplicatesRequest, PresentDuplicatesResponse>(request);

        Dispatcher.UIThread.Post(() =>
        {
            IOrderedEnumerable<DuplicateGroupListItem> listItems = response.Duplicates
                .Select(x => new DuplicateGroupListItem(x))
                .OrderByDescending(x => x.DuplicateGroup.FileSize);
            DuplicateGroups.AddRange(listItems);

            SelectedDuplicateGroup = IdentifyDuplicateGroup(response.CurrentDuplicateGroup);

            FooterViewModel.SetDuplicateGroupCount(DuplicateGroups.Count);
            FooterViewModel.SetTotalSize(response.TotalSize);
        });
        Dispatcher.UIThread.RunJobs();
    }

    private DuplicateGroupListItem IdentifyDuplicateGroup(DuplicateGroup duplicateGroup)
    {
        return duplicateGroup == null
            ? null
            : DuplicateGroups.FirstOrDefault(x => x.DuplicateGroup.FileHash == duplicateGroup.FileHash);
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