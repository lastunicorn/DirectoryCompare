// Directory Compare
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

using System.Diagnostics;
using MediatR;

namespace DustInTheWind.Clindy.Applications.OpenInExplorer;

internal class OpenInExplorerUseCase : IRequestHandler<OpenInExplorerRequest>
{
    public Task Handle(OpenInExplorerRequest request, CancellationToken cancellationToken)
    {
        // todo: Create ports and adapters for this code.
        
        bool fileExists = File.Exists(request.FilePath);

        if (fileExists)
        {
            Process process = new();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = "nautilus",
                Arguments = @$"""{request.FilePath}""",
                WindowStyle = ProcessWindowStyle.Hidden
            };
            process.Start();
        }

        return Task.CompletedTask;
    }
}