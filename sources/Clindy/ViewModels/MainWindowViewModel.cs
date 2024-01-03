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

using System.Collections.Generic;
using System.Linq;
using DustInTheWind.DirectoryCompare.DataStructures;
using ReactiveUI;

namespace DustInTheWind.Clindy.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private DuplicateGroupListItem selectedDuplicateGroup;
    private List<DuplicateFilesListItem> duplicateFiles;

    public string Greeting => "Welcome to Avalonia!";

    public List<DuplicateGroupListItem> DuplicateGroups { get; }

    public DuplicateGroupListItem SelectedDuplicateGroup
    {
        get => selectedDuplicateGroup;
        set
        {
            this.RaiseAndSetIfChanged(ref selectedDuplicateGroup, value);

            DuplicateFiles = value.DuplicateGroup.FilePaths
                .Select(x => new DuplicateFilesListItem(x))
                .ToList();
        }
    }

    public List<DuplicateFilesListItem> DuplicateFiles
    {
        get => duplicateFiles;
        set => this.RaiseAndSetIfChanged(ref duplicateFiles, value);
    }

    public MainWindowViewModel()
    {
        List<DuplicateGroup> duplicateGroups = new()
        {
            new DuplicateGroup
            {
                FileSize = 1023424,
                FileHash = FileHash.Empty,
                FilePaths = new List<string>
                {
                    "/nfs/YubabaData/Alez/Untitled Document 1.txt",
                    "/nfs/YubabaData/Alez/Backup/Untitled Document 1.txt",
                    "/nfs/YubabaData/Alez/to be sorted/Untitled Document 1.txt"
                }
            },
            new DuplicateGroup
            {
                FileSize = 8442,
                FileHash = FileHash.Empty,
                FilePaths = new List<string>
                {
                    "/nfs/YubabaData/Alez/gsm-solution.png",
                    "/nfs/YubabaData/Alez/Backup/gsm-solution.png",
                }
            },
            new DuplicateGroup
            {
                FileSize = 1023424,
                FileHash = FileHash.Empty,
                FilePaths = new List<string>
                {
                    "/nfs/YubabaData/Alez/20150212091347191.ods",
                    "/nfs/YubabaData/Alez/Backup/document.ods",
                    "/nfs/YubabaData/Alez/to be sorted/20150212091347191.ods",
                    "/nfs/YubabaData/Alez/to be sorted 2/20150212091347191.ods",
                    "/nfs/YubabaData/Alez/Temp/2023 12 17.ods"
                }
            }
        };

        DuplicateGroups = duplicateGroups
            .Select(x => new DuplicateGroupListItem(x))
            .ToList();
    }
}