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

using DustInTheWind.DirectoryCompare.Cli.Application.MiscellaneousArea.CompareSnapshots;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.MiscellaneousCommands.CompareSnapshots;

public class CompareViewModel
{
    public IReadOnlyList<string> OnlyInSnapshot1 { get; }

    public IReadOnlyList<string> OnlyInSnapshot2 { get; }

    public IReadOnlyList<FilePairDto> DifferentNames { get; }

    public IReadOnlyList<FilePairDto> DifferentContent { get; }

    public string ExportDirectoryPath { get; }

    public CompareViewModel(CompareSnapshotsResponse response)
    {
        OnlyInSnapshot1 = response.OnlyInSnapshot1;
        OnlyInSnapshot2 = response.OnlyInSnapshot2;
        DifferentNames = response.DifferentNames;
        DifferentContent = response.DifferentContent;
        ExportDirectoryPath = response.ExportDirectoryPath;
    }
}