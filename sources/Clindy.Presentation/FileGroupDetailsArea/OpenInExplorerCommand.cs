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

using System.Windows.Input;
using DustInTheWind.Clindy.Applications;
using DustInTheWind.Clindy.Applications.OpenInExplorer;

namespace DustInTheWind.Clindy.Presentation.FileGroupDetailsArea;

public class OpenInExplorerCommand : ICommand
{
    private readonly RequestBus requestBus;

    public event EventHandler CanExecuteChanged;

    public OpenInExplorerCommand(RequestBus requestBus)
    {
        this.requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
    }

    public bool CanExecute(object parameter)
    {
        if (parameter is FileGroupItem item)
        {
            string filePath = item.FilePath;
            return File.Exists(filePath);
        }

        return false;
    }

    public void Execute(object parameter)
    {
        if (parameter is FileGroupItem item)
        {
            OpenInExplorerRequest request = new()
            {
                FilePath = item.FilePath
            };
            _ = requestBus.PlaceRequest(request);
        }
    }
}