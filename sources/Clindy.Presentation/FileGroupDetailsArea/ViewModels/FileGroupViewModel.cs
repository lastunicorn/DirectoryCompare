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

using DustInTheWind.Clindy.Applications;
using DustInTheWind.Clindy.Applications.SetCurrentDuplicateGroup;
using DustInTheWind.DirectoryCompare.Infrastructure;
using ReactiveUI;

namespace DustInTheWind.Clindy.Presentation.FileGroupDetailsArea.ViewModels;

public class FileGroupViewModel : ViewModelBase
{
    private List<FileGroupItem> duplicateFiles;
    private readonly OpenInExplorerCommand openInExplorerCommand;

    public FileGroupViewModel(EventBus eventBus, OpenInExplorerCommand openInExplorerCommand)
    {
        if (eventBus == null) throw new ArgumentNullException(nameof(eventBus));
        this.openInExplorerCommand = openInExplorerCommand ?? throw new ArgumentNullException(nameof(openInExplorerCommand));

        eventBus.Subscribe<CurrentDuplicateGroupChangedEvent>(HandleCurrentDuplicateChangedEvent);
    }
    
    public List<FileGroupItem> DuplicateFiles
    {
        get => duplicateFiles;
        set => this.RaiseAndSetIfChanged(ref duplicateFiles, value);
    }

    private void HandleCurrentDuplicateChangedEvent(CurrentDuplicateGroupChangedEvent ev)
    {
        DuplicateFiles = ev.DuplicateGroup?.FilePaths
            .Select(x => new FileGroupItem
            {
                FilePath = x,
                OpenCommand = openInExplorerCommand
            })
            .ToList();
    }
}