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

using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.DirectoryCompare.Cli.Application.MiscellaneousArea.PresentDuplicates;
using MediatR;

namespace DustInTheWind.DirectoryCompare.Cli.Presentation.MiscellaneousCommands.ShowDuplicates;

[NamedCommand("show-duplicates", Description = "Display the content of a json file containing previously exported duplicates.")]
internal class ShowDuplicatesCommand : IConsoleCommand<ShowDuplicatesViewModel>
{
    private readonly IMediator mediator;

    [NamedParameter("file", ShortName = 'f', IsOptional = false, Description = "The path to the file containing the duplicates results.")]
    public string FilePath { get; set; }

    [NamedParameter("check-exist", ShortName = 'x', IsOptional = true, Description = "If specified, the files that does not actually exist on disk are not displayed.")]
    public bool CheckFilesExistence { get; set; }

    public ShowDuplicatesCommand(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }
    
    public async Task<ShowDuplicatesViewModel> Execute()
    {
        PresentDuplicatesRequest request = new()
        {
            FilePath = FilePath,
            CheckFilesExistence = CheckFilesExistence
        };
        PresentDuplicatesResponse response = await mediator.Send(request);
        
        return new ShowDuplicatesViewModel
        {
            FileGroups = response.Duplicates,
            DuplicateCount = response.DuplicateCount,
            TotalSize = response.TotalSize
        };
    }
}