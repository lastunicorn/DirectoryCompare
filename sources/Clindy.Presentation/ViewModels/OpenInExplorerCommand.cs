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

using System.Diagnostics;
using System.Windows.Input;

namespace DustInTheWind.Clindy.Presentation.ViewModels;

public class OpenInExplorerCommand : ICommand
{
    private readonly string filePath;

    public event EventHandler? CanExecuteChanged;

    public OpenInExplorerCommand(string filePath)
    {
        this.filePath = filePath;
    }

    public bool CanExecute(object? parameter)
    {
        return File.Exists(filePath);
    }

    public void Execute(object? parameter)
    {
        Process process = new();
        process.StartInfo = new ProcessStartInfo
        {
            FileName = "nautilus",
            Arguments = @$"""{filePath}""",
            WindowStyle = ProcessWindowStyle.Hidden
        };
        process.Start();
    }
}