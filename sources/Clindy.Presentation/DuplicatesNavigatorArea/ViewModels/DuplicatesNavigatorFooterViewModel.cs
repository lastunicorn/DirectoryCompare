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

using DustInTheWind.DirectoryCompare.DataStructures;
using ReactiveUI;

namespace DustInTheWind.Clindy.Presentation.DuplicatesNavigatorArea.ViewModels;

public class DuplicatesNavigatorFooterViewModel : ViewModelBase
{
    private int duplicateGroupCount;
    private string totalSize;

    public int DuplicateGroupCount
    {
        get => duplicateGroupCount;
        private set => this.RaiseAndSetIfChanged(ref duplicateGroupCount, value);
    }

    public string TotalSize
    {
        get => totalSize;
        private set => this.RaiseAndSetIfChanged(ref totalSize, value);
    }

    public void SetDuplicateGroupCount(int value)
    {
        DuplicateGroupCount = value;
    }

    public DuplicatesNavigatorFooterViewModel()
    {
        Clear();
    }

    public void SetTotalSize(DataSize value)
    {
        if (value < DataSize.OneKilobyte)
            TotalSize = value.ToString("simple");
        else
            TotalSize = value.ToString("detailed");
    }

    public void Clear()
    {
        SetDuplicateGroupCount(0);
        SetTotalSize(DataSize.Zero);
    }
}