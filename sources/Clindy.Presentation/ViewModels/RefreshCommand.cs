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

using System.Windows.Input;
using DustInTheWind.Clindy.Applications;
using DustInTheWind.Clindy.Applications.LoadDuplicates;
using DustInTheWind.Clindy.Applications.Refresh;

namespace DustInTheWind.Clindy.Presentation.ViewModels;

public class RefreshCommand : ICommand
{
    private readonly RequestBus requestBus;
    private bool canExecute = true;

    public event EventHandler CanExecuteChanged;

    public RefreshCommand(RequestBus requestBus, EventBus eventBus)
    {
        if (eventBus == null) throw new ArgumentNullException(nameof(eventBus));
        this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));

        eventBus.Subscribe<DuplicatesListLoadingEvent>(HandleDuplicatesListLoadingEvent);
        eventBus.Subscribe<DuplicatesListLoadedEvent>(HandleDuplicatesListLoadedEvent);
    }

    private Task HandleDuplicatesListLoadingEvent(DuplicatesListLoadingEvent ev, CancellationToken cancellationToken)
    {
        canExecute = false;
        OnCanExecuteChanged();

        return Task.CompletedTask;
    }

    private Task HandleDuplicatesListLoadedEvent(DuplicatesListLoadedEvent ev, CancellationToken cancellationToken)
    {
        canExecute = true;
        OnCanExecuteChanged();

        return Task.CompletedTask;
    }

    public bool CanExecute(object parameter)
    {
        return canExecute;
    }

    public void Execute(object parameter)
    {
        RefreshRequest request = new();
        _ = requestBus.PlaceRequest(request);
    }

    protected virtual void OnCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}