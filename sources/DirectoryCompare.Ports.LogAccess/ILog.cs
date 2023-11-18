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

namespace DustInTheWind.DirectoryCompare.Ports.LogAccess;

public interface ILog
{
    void WriteDebug(string message);

    void WriteDebug(string format, params object[] args);

    void WriteInfo(string message);

    void WriteInfo(string format, params object[] args);

    void WriteWarning(string message);

    void WriteWarning(string format, params object[] args);

    void WriteWarning(string message, Exception ex);

    void WriteWarning(Exception ex);

    void WriteError(string message);

    void WriteError(string format, params object[] args);

    void WriteError(string message, Exception ex);

    void WriteError(Exception ex);
}