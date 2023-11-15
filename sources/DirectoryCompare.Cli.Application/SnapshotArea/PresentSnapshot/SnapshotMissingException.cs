// VeloCity
// Copyright (C) 2022-2023 Dust in the Wind
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

using System.Runtime.Serialization;
using DustInTheWind.DirectoryCompare.DataStructures;

namespace DustInTheWind.DirectoryCompare.Cli.Application.SnapshotArea.PresentSnapshot;

[Serializable]
public class SnapshotMissingException : Exception
{
    private const string DefaultMessage = "The snapshot '{0}' could not be found.";

    public SnapshotMissingException(SnapshotLocation snapshotLocation)
        : base(BuildMessage(snapshotLocation))
    {
    }

    private static string BuildMessage(SnapshotLocation snapshotLocation)
    {
        return string.Format(DefaultMessage, snapshotLocation);
    }

    public SnapshotMissingException(SerializationInfo serializationInfo, StreamingContext streamingContext)
        : base(serializationInfo, streamingContext)
    {
    }
}